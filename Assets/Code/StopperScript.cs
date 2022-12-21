using UnityEngine;

public class StopperScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GameManagerScript.CurrentPlayer.BallScript.StopBall();
    }
}