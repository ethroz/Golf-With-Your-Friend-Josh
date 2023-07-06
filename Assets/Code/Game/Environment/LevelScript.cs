using System;
using UnityEngine;
using static UnityEngine.Extensions;

public class LevelScript : MonoBehaviour, IGameListener, IComparable<LevelScript>
{
    public GameManagerScript Game { get; private set; }
    public int Par = 1;
    public int LevelIndex { get; private set; }
    public FinishScript Finish => finish;
    public PoleScript Pole => pole;
    public int[] PlayerStats { get; private set; }

    private PoleScript pole;
    private FinishScript finish;

    private void Awake()
    {
        // get the last one or two characters from the name and parse them as the level index + 1
        LevelIndex = int.Parse(gameObject.name[6..]) - 1;

        this.GetComponentInChildrenOrThrow(out pole);
        this.GetComponentInChildrenOrThrow(out finish);
    }

    private void Start()
    {
        finish.SetManager(Game);
    }

    public void SetManager(GameManagerScript manager)
    {
        Game = manager;
    }

    public void SetStatsFromPlayers(Player[] players)
    {
        PlayerStats = new int[players.Length];
        for (int i = 0; i < PlayerStats.Length; ++i)
        {
            PlayerStats[i] = players[i].Score;
        }
    }

    public int CompareTo(LevelScript other)
    {
        return LevelIndex - other.LevelIndex;
    }
}
