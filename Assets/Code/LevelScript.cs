using UnityEngine;

public class LevelScript : MonoBehaviour
{
    public int Par = 1;
    public int LevelIndex { get; private set; }
    public FinishScript Finish { get; private set; }
    public PoleScript Pole { get; private set; }
    public int[] PlayerStats { get; set; }

    private void Awake()
    {
        // get the last one or two characters from the name and parse them as the level index + 1
        LevelIndex = int.Parse(gameObject.name.Substring(6)) - 1;

        Finish = GetComponentInChildren<FinishScript>();
        Pole = GetComponentInChildren<PoleScript>();
    }

    public void SetStats()
    {
        PlayerStats = new int[GameManagerScript.Players.Count];
        for (int i = 0; i < PlayerStats.Length; i++)
        {
            PlayerStats[i] = GameManagerScript.Players[i].Score;
        }
    }
}