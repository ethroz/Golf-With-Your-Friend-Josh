using UnityEngine;

public class FinishScript : MonoBehaviour
{
    private ParticleSystem PS;

    private void Awake()
    {
        PS = gameObject.GetComponentInParent<GoalTag>().gameObject.GetComponentInChildren<ParticleSystem>();
    }

    private void OnTriggerEnter(Collider other)
    {
        GameManagerScript.CurrentPlayer.Done = true;
        GameManagerScript.NumberDone++;
        PS.Play();
    }
}