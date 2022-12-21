using UnityEngine;

public class BladeScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GameManagerScript.CurrentPlayer.BallScript.Dead = true;
    }
}