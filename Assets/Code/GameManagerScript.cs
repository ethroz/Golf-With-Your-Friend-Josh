using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    // Assignments
    public GameObject[] CharacterList;
    public GameObject[] ClubList;
    public GameObject[] BallList;

    // General
    public static GameObject[] Characters;
    public static GameObject[] Clubs;
    public static GameObject[] Balls;
    public static LevelScript[] LevelScripts { get; private set; }
    public static int LevelIndex
    {
        get => _levelIndex;
        set
        {
            if (LevelIndex != value)
                PreviousLevelIndex = _levelIndex;
            _levelIndex = value;
        }
    }
    private static int _levelIndex;
    private static int PreviousLevelIndex;
    public static int PlayerIndex { get; private set; }
    public static int NumberDone { get; set; }
    public static List<Player> Players { get; private set; }
    public static Player CurrentPlayer => Players[PlayerIndex];
    public static LevelScript CurrentLevel => LevelScripts[LevelIndex];
    public static GameObject CurrentCharacter => CurrentPlayer.Character;
    public static GameObject CurrentClub => CurrentPlayer.Club;
    public static GameObject CurrentBall => CurrentPlayer.Ball;
    public static Transform TeePosition { get; private set; }
    public static Vector3 MenuStartPosition = new Vector3(44.0f, 34.0f, 52.0f);
    public static Vector3 MenuPivot = new Vector3(-40.0f, 0.0f, 43.0f);

    // States
    public enum GameState { Menu, LocalSolo, LocalVersus, OnlinePlay };
    public enum MenuState { Main, Controls, Settings, LevelSelect, Lobby, Pause, Game };
    public enum PlayerState { Direction, Power, Watch, Result, Spectate, Menu }
    public static GameState CurrentGameState { get; private set; }
    public static MenuState CurrentMenuState { get; private set; }
    public static Stack<MenuState> LastMenuStates { get; private set; }

    // private fields
    private static Transform cam;
    private static ReflectionProbe reflectionProbe;
    private static Queue<int> startOrder;

    // literal fields
    public static float Sensitivity
    {
        set
        {
            _sensitivity = value;
        }
        get => _sensitivity * 10.0f;
    }
    private static float _sensitivity = 0.160f;

    // field and property declarations + error checks
    private void Awake()
    {
#if UNITY_EDITOR
        Application.targetFrameRate = 0;
#endif
        if (CharacterList.Length == 0 || ClubList.Length == 0 || BallList.Length == 0)
        {
            Debug.LogError("Game Manager Missing Entities");
            ExitGame();
        }
        Characters = CharacterList;
        Clubs = ClubList;
        Balls = BallList;

        AssignLevels();

        Players = new List<Player>();
        PlayerIndex = 0;
        AddNewPlayer(Camera.main.GetComponent<LocalPlayerScript>(), 0, 0, 0);
        LastMenuStates = new Stack<MenuState>();
        cam = Camera.main.transform;
        TeePosition = GameObject.FindGameObjectWithTag("GameController").transform;
        reflectionProbe = TeePosition.GetComponentInChildren<ReflectionProbe>();
    }

    // set states
    private void Start()
    {
        SetGameState(GameState.Menu);
        SetMenuState(MenuState.Main);
    }

    // Instantiate the players' gameobjects and deactivate them
    private static void AddNewPlayer(Player p, int CharIndex, int ClubIndex, int BallIndex)
    {
        p.Character = Instantiate(Characters[CharIndex], new Vector3(), new Quaternion());
        p.Club = Instantiate(Clubs[ClubIndex], new Vector3(), new Quaternion());
        p.Ball = Instantiate(Balls[BallIndex], new Vector3(), new Quaternion());
        p.Character.SetActive(false);
        p.Club.SetActive(false);
        p.Ball.SetActive(false);
        Players.Add(p);
    }

    private static void AssignLevels()
    {
        GameObject[] levels = GameObject.FindGameObjectsWithTag("Level");
        LevelScripts = new LevelScript[levels.Length];
        for (int i = 0; i < levels.Length; i++)
        {
            GameObject currentLevel = null;
            for (int j = 0; j < levels.Length; j++)
            {
                if (levels[j].name == "Level " + (i + 1))
                    currentLevel = levels[j];
            }
            LevelScripts[i] = currentLevel.GetComponent<LevelScript>();
        }
    }

    public static void SetMenuState(MenuState state)
    {
        switch (state)
        {
            case MenuState.Main:
                Time.timeScale = 1.0f;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                LevelIndex = -1;
                UIScript.EnableMenu(0);
                LastMenuStates.Clear();
                SetCurrentPlayerState(PlayerState.Menu);
                break;
            case MenuState.Controls:
                UIScript.EnableMenu(3);
                break;
            case MenuState.Settings:
                UIScript.EnableMenu(2);
                break;
            case MenuState.LevelSelect:
                UIScript.EnableMenu(4);
                break;
            case MenuState.Pause:
                Time.timeScale = 0.0f;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                UIScript.EnableMenu(1);
                SetCurrentPlayerState(PlayerState.Menu);
                break;
            case MenuState.Game:
                Time.timeScale = 1.0f;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                UIScript.EnableMenu(5);
                break;
            default:
                throw new NotImplementedException();
        }
        CurrentMenuState = state;
    }

    public static void SetUIFromPlayerState()
    {
        switch (Players[0].State)
        {
            case PlayerState.Menu:
                UIScript.DeactivatePowerBar();
                UIScript.SetContinueObject(false);
                break;
            case PlayerState.Power:
                UIScript.ActivatePowerBar();
                break;
            case PlayerState.Watch:
                UIScript.DeactivatePowerBar(0.5f);
                break;
            case PlayerState.Result:
                UIScript.SetContinueObject(true);
                break;
            case PlayerState.Spectate:
                UIScript.SetContinueObject(false);
                break;
        }
    }

    public static void SetGameState(GameState state)
    {
        switch (state)
        {
            case GameState.Menu:
                HideAllPlayers();
                MainMenuCamera();
                break;
            case GameState.LocalSolo:
                CurrentClub.SetActive(true);
                CurrentBall.SetActive(true);
                BakeReflections();
                SetCurrentPlayerState(PlayerState.Direction);
                break;
            default:
                throw new NotImplementedException();
        }
        CurrentGameState = state;
    }

    public static void SetCurrentPlayerState(PlayerState state)
    {
        CurrentPlayer.SetState(state);
        if (state == PlayerState.Spectate)
        {
            if (NumberDone == Players.Count)
            {
                CurrentLevel.SetStats();
                SetLevel(LevelIndex + 1);
            }
            SetNextPlayer();
            BakeReflections();
            CurrentPlayer.SetState(PlayerState.Direction);
        }
        SetUIFromPlayerState();
    }

    public static void IncrementPlayerState()
    {
        SetCurrentPlayerState((PlayerState)(((int)CurrentPlayer.State + 1) % 5));
    }

    private static void SetNextPlayer()
    {
        // turns according to stats from last round
        if (startOrder.Count > 0)
        {
            PlayerIndex = startOrder.Dequeue();
            return;
        }

        // distance based turns
        int worstIndex = 0;
        float worstDistanceSqr = 0.0f;
        for (int i = 0; i < Players.Count; i++)
        {
            if (Players[i].Done)
                continue;
            float distSqr = (Players[i].Ball.transform.position - CurrentLevel.transform.position).sqrMagnitude;
            if (distSqr > worstDistanceSqr)
            {
                worstDistanceSqr = distSqr;
                worstIndex = i;
            }
        }
        PlayerIndex = worstIndex;
    }

    public static void SetLevel(int level)
    {
        LevelIndex = level;
        NumberDone = 0;
        ResetAllPlayers();
        SetStartOrder();
        UIScript.SetPar();
        UIScript.SetScore();
    }

    private static void HideAllPlayers()
    {
        foreach (Player p in Players)
        {
            p.Character.SetActive(false);
            p.Club.SetActive(false);
            p.Ball.SetActive(false);
        }
    }

    private static void ShowCurrentPlayer()
    {
        CurrentCharacter.SetActive(true);
        CurrentClub.SetActive(true);
        CurrentBall.SetActive(true);
    }

    // find the position for character
    private static void ResetAllPlayers()
    {
        foreach (Player p in Players)
        {
            // direction
            p.Pitch = 33.0f;
            p.Yaw = CurrentLevel.transform.rotation.eulerAngles.y;
            // ball
            p.BallScript.MoveBall(CurrentLevel.transform.position + Vector3.up * p.BallScript.Radius);
            // club
            p.Club.transform.position = p.Ball.transform.position + new Vector3(0.032f, 1.747f, -0.104f);
            p.Club.transform.eulerAngles = new Vector3();
            p.Club.transform.RotateAround(p.Ball.transform.position, Vector3.up, p.Yaw);
            // character
            p.Character.transform.position = CurrentLevel.transform.position - 1.5f * CurrentLevel.transform.forward - 1.5f * CurrentLevel.transform.right;
            p.Character.transform.eulerAngles = new Vector3(0.0f, 45.0f + p.Yaw, 0.0f);
            // others
            p.Done = false;
            p.Score = -CurrentLevel.Par;
        }
    }

    private static void SetStartOrder()
    {
        // numerical order
        int[] list = new int[Players.Count];
        for (int i = 0; i < list.Length; i++)
        {
            list[i] = i;
        }

        // check if the players have a previous round
        if (PreviousLevelIndex >= 0)
        {
            // sort the players by best score from the previous round
            int[] sorted = new int[list.Length];
            for (int i = 0; i < list.Length; i++)
                sorted[i] = LevelScripts[PreviousLevelIndex].PlayerStats[i];
            Array.Sort(sorted, list);
        }
        startOrder = new Queue<int>(list);
    }

    public static void BackToStart()
    {
        startOrder.Enqueue(PlayerIndex);
    }

    public static void FlashPole()
    {
        CurrentLevel.Pole.FlashOn();
    }

    public static void StopFlashPole()
    {
        if (LevelIndex >= 0)
            CurrentLevel.Pole.FlashOff();
    }

    private static void MainMenuCamera()
    {
        cam.position = MenuStartPosition;
        cam.LookAt(MenuPivot);
    }

    private static void BakeReflections()
    {
        if (Physics.Raycast(CurrentBall.transform.position, Vector3.down, out RaycastHit rhit, 1.0f, BallScript.LayerMask))
        {
            TeePosition.position = rhit.point;
            TeePosition.eulerAngles = rhit.transform.eulerAngles;
        }
        else
        {
            TeePosition.position = CurrentBall.transform.position + Vector3.down * CurrentPlayer.BallScript.Radius;
            TeePosition.eulerAngles = CurrentLevel.transform.eulerAngles;
        }
        reflectionProbe.RenderProbe();
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.P))
        {
            UnityEditor.EditorApplication.isPaused = !UnityEditor.EditorApplication.isPaused;
        }
#endif
        if (Input.GetButtonDown("Cancel"))
        {
            if (CurrentMenuState == MenuState.Main)
            {
                ExitGame();
            }
            else if (CurrentMenuState == MenuState.Game)
            {
                SetMenuState(MenuState.Pause);
            }
            else
            {
                UIScript.BackButton();
            }
        }
        UIScript.CheckOff();
    }

    public static string V2S(Vector3 v)
    {
        return "(" + v.x.ToString() + ", " + v.y.ToString() + ", " + v.z.ToString() + ")";
    }

    public static void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}