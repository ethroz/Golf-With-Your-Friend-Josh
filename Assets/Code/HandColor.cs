using UnityEngine;

public class HandColor : MonoBehaviour
{
    private int playerIndex;
    private int levelIndex;
    private Vector3 goal;
    private float distance;
    private Material material;
    private Color minColor = new Color(500.0f, 0.0f, 0.0f);
    private Color maxColor = new Color(10000.0f, 0.0f, 0.0f);

    private void Start()
    {
        material = gameObject.GetComponent<Renderer>().material;
    }

    private void Update()
    {
        if (CheckForUpdates())
            return;
        Vector3 ball = GameManagerScript.Players[playerIndex].Ball.transform.position;
		float dist = (goal - ball).magnitude;
        float frac = 1.0f - Mathf.Clamp(dist / distance, 0.0f, 1.0f);
        material.SetColor("_EmissiveColor", Color.Lerp(minColor, maxColor, Mathf.Pow(frac, 7.0f)));
    }

    private bool CheckForUpdates()
    {
        if (GameManagerScript.LevelIndex == -1)
            return true;
        if (playerIndex != GameManagerScript.PlayerIndex)
        {
            playerIndex = GameManagerScript.PlayerIndex;
        }
        if (levelIndex != GameManagerScript.LevelIndex)
        {
            levelIndex = GameManagerScript.LevelIndex;
            goal = GameManagerScript.CurrentLevel.Finish.transform.position;
            distance = (goal - GameManagerScript.CurrentLevel.transform.position).magnitude;
        }
        return false;
    }
}