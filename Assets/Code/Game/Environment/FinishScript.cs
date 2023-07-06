using UnityEngine;
using static UnityEngine.Extensions;

public class FinishScript : MonoBehaviour, IGameListener
{
    public GameManagerScript Game { get; private set; }

    private ParticleSystem PS;

    private void Awake()
    {
        this.GetComponentInParentOrThrow(out GoalTag goal);
        goal.GetComponentInChildrenOrThrow(out PS);
    }

    private void OnTriggerEnter(Collider other)
    {
        Game.CurrentPlayer.Done = true;
        PS.Play();
    }

    public void SetManager(GameManagerScript manager)
    {
        Game = manager;
    }
}
