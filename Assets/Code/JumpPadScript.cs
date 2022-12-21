using UnityEngine;

public class JumpPadScript : MonoBehaviour
{
    public float BoostAmount = 3.0f;

    private void OnTriggerStay(Collider other)
    {
        GameManagerScript.CurrentPlayer.BallScript.Impulse(transform.up * BoostAmount);
    }
}