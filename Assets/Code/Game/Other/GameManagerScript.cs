using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using States;
using static UnityEngine.Extensions;

public class GameManagerScript : MonoBehaviour
{
    private static int numInstances = 0;

    // Assignable
    public GameObject[] Characters;
    public GameObject[] Clubs;
    public GameObject[] Balls;

    // Shortcuts
    public Player CurrentPlayer => players[playerIndex];
    public LevelScript CurrentLevel => levels[LevelIndex];
    public GameObject CurrentCharacter => CurrentPlayer.Character;
    public GameObject CurrentClub => CurrentPlayer.Club;
    public GameObject CurrentBall => CurrentPlayer.Ball;
    public int LevelIndex { get; private set; }
    public SettingsManagerScript Settings { get; private set; }
    public UIScript UI { get; private set; }

    // General
    private GameState State { get => state; set { prevState = state; state = value; } }
    private GameState state;
    private GameState prevState;
    private int controller;
    private List<Player> players;
    private LevelScript[] levels;
    private int playerIndex;
    private ReflectionProbe reflectionProbe;
    private static Queue<int> startOrder;

    // Definitions
    public const int NUM_LEVELS = 18;
    private static readonly Vector3 menuCameraStartPosition = new(44.0f, 34.0f, 52.0f);
    private static readonly Vector3 menuCameraPivotPoint = new(-40.0f, 0.0f, 43.0f);
    private static readonly float menuCameraPivotSpeed = -3.0f;

    private void Awake()
    {
        if (++numInstances > 1)
        {
            throw new NotSupportedException("Cannot have more than one game manager");
        }
        if (Characters.Length == 0 || Clubs.Length == 0 || Balls.Length == 0)
        {
            throw new MissingReferenceException("Game Manager missing prefabs");
        }

        UI = GetOrThrow<UIScript>();
        UI.SetManager(this);

        Settings = GetOrThrow<SettingsManagerScript>();
        Settings.SetManager(this);

        players = new();
        var localPlayer = GetOrThrow<LocalPlayerScript>();
        AddNewPlayer(localPlayer, 0, 0, 0);
        playerIndex = 0;

        foreach (var ball in Balls)
        {
            ball.AssertHasComponent<BallScript>();
        }
        foreach (var club in Clubs)
        {
            club.AssertHasComponent<ClubScript>();
        }

        this.GetComponentInChildrenOrThrow(out reflectionProbe);

#if UNITY_EDITOR
        Application.targetFrameRate = 144;
#endif
    }

    private void Start()
    {
        // Late setup because the level scripts need to determine their level numbers.
        levels = GetOrThrow<LevelScript>(NUM_LEVELS);
        Array.Sort(levels);
        for (int i = 0; i < NUM_LEVELS; ++i)
        {
            if (i != levels[i].LevelIndex)
            {
                throw new MissingReferenceException("Missing Level " + i);
            }
            levels[i].SetManager(this);
        }

        SetState(GameState.Main);
    }

    private void Update()
    {
        if (State == GameState.Main)
        {
            Camera.main.transform.RotateAround(menuCameraPivotPoint, Vector3.up, menuCameraPivotSpeed * Time.deltaTime);
        }
    }

    private void LateUpdate()
    {
        for (int i = 0; i < players.Count; ++i)
        {
            players[i].SetControl(i == controller);
        }
    }

    public void SetPrevState()
    {
        SetState(prevState);
    }

    public void SetState(GameState state)
    {
        switch (state)
        {
            case GameState.Main:
                Time.timeScale = 1.0f;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                HideAllPlayers();
                SetupMainMenuCamera();
                SetControl(-1);
                UI.SetState(MenuState.Main);
                break;
            case GameState.Paused:
                Time.timeScale = 0.0f;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                SetControl(-1);
                UI.SetState(MenuState.Pause);
                break;
            case GameState.LocalSolo:
                Time.timeScale = 1.0f;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                SetControl(playerIndex);
                break;
            default:
                throw new NotImplementedException();
        }
        State = state;
    }

    public void ResetLevel(int level)
    {
        LevelIndex = level;
        ResetPlayerPositions();
        SetStartOrder();
        playerIndex = startOrder.Dequeue();
        ShowCurrentPlayer();
        CurrentPlayer.SetState(PlayerState.Direction);
        FlashPole();
    }

    public void FlashPole()
    {
        if (LevelIndex >= 0)
        {
            CurrentLevel.Pole.FlashOn();
        }
    }

    public void StopFlashPole()
    {
        if (LevelIndex >= 0)
        {
            CurrentLevel.Pole.FlashOff();
        }
    }

    public void NextPlayer()
    {
        bool allDone = true;
        for (int i = 0; i < players.Count; ++i)
        {
            if (!players[i].Done)
            {
                allDone = false;
                break;
            }
        }

        if (allDone)
        {
            // Next level.
            CurrentLevel.SetStatsFromPlayers(players.ToArray());
            ResetLevel(++LevelIndex);
        }
        else
        {
            // turns according to stats from last round
            if (startOrder.Count > 0)
            {
                playerIndex = startOrder.Dequeue();
            }
            else
            {
                // distance based turns
                int worstIndex = -1;
                float worstDistanceSqr = 0.0f;
                for (int i = 0; i < players.Count; ++i)
                {
                    if (players[i].Done)
                        continue;
                    float distSqr = (players[i].Ball.transform.position - CurrentLevel.transform.position).sqrMagnitude;
                    if (distSqr + players[i].Score > worstDistanceSqr)
                    {
                        worstDistanceSqr = distSqr;
                        worstIndex = i;
                    }
                }
                Trace.Assert(worstIndex > 0);
                playerIndex = worstIndex;
            }
        }

        ShowCurrentPlayer();
        BakeReflections();
        CurrentPlayer.SetState(PlayerState.Direction);
    }

    private void AddNewPlayer(Player p, int CharIndex, int ClubIndex, int BallIndex)
    {
        p.SetControl(false);
        p.SetManager(this);
        p.Character = Instantiate(Characters[CharIndex]);
        p.Club = Instantiate(Clubs[ClubIndex]);
        p.Ball = Instantiate(Balls[BallIndex]);
        p.Character.SetActive(false);
        p.Club.SetActive(false);
        p.Ball.SetActive(false);
        players.Add(p);
    }

    private void SetControl(int index)
    {
        controller = index;
    }

    private void HideAllPlayers()
    {
        foreach (Player p in players)
        {
            p.Character.SetActive(false);
            p.Club.SetActive(false);
            p.Ball.SetActive(false);
            p.SetControl(false);
        }
    }

    private void ShowCurrentPlayer()
    {
        for (int i = 0; i < players.Count; ++i)
        {
            players[i].Character.SetActive(i == playerIndex);
            players[i].Club.SetActive(i == playerIndex);
            players[i].Ball.SetActive(i == playerIndex);
        }

        SetControl(playerIndex);
    }

    private void ResetPlayerPositions()
    {
        foreach (Player p in players)
        {
            p.NewLevel(CurrentLevel.transform);
        }
    }

    private void SetStartOrder()
    {
        // numerical order
        int[] list = new int[players.Count];
        for (int i = 0; i < list.Length; ++i)
        {
            list[i] = i;
        }

        // check if the players have a previous round
        if (LevelIndex > 0 && levels[LevelIndex - 1].PlayerStats != null)
        {
            // sort the players by best score from the previous round
            int[] sorted = new int[list.Length];
            for (int i = 0; i < list.Length; ++i)
                sorted[i] = levels[LevelIndex - 1].PlayerStats[i];
            Array.Sort(sorted, list);
        }
        startOrder = new(list);
    }

    private void SetupMainMenuCamera()
    {
        Camera.main.transform.position = menuCameraStartPosition;
        Camera.main.transform.LookAt(menuCameraPivotPoint);
    }

    private void BakeReflections()
    {
        if (Physics.Raycast(CurrentBall.transform.position, Vector3.down, out RaycastHit rHit, 1.0f, BallScript.LayerMask))
        {
            reflectionProbe.transform.position = rHit.point;
        }
        else
        {
            reflectionProbe.transform.position = CurrentBall.transform.position;
        }
        reflectionProbe.RenderProbe();
    }
}
