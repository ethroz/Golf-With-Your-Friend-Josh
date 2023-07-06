using UnityEngine;
using static UnityEngine.Extensions;

[RequireComponent(typeof(Renderer))]
public class HandColorScript : MonoBehaviour
{
    private GameManagerScript game;
    private int levelIndex = -1;
    private Vector3 goal;
    private float distance;
    private Material material;
    private Color minColor = new(500.0f, 0.0f, 0.0f);
    private Color maxColor = new(10000.0f, 0.0f, 0.0f);

    private void Start()
    {
        this.GetComponentOrThrow(out Renderer renderer);
        material = renderer.material;
        game = GetOrThrow<GameManagerScript>();
    }

    private void Update()
    {
        if (!HasUpdates())
            return;
        Vector3 ball = game.CurrentBall.transform.position;
		float dist = (goal - ball).magnitude;
        float frac = 1.0f - Mathf.Clamp(dist / distance, 0.0f, 1.0f);
        material.SetColor("_EmissiveColor", Color.Lerp(minColor, maxColor, Mathf.Pow(frac, 7.0f)));
    }

    private bool HasUpdates()
    {
        if (levelIndex != game.LevelIndex)
        {
            levelIndex = game.LevelIndex;
            goal = game.CurrentLevel.Finish.transform.position;
            distance = (goal - game.CurrentLevel.transform.position).magnitude;
        }
        return true;
    }
}
