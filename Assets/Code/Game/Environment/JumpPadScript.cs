using UnityEngine;

public class JumpPadScript : MonoBehaviour
{
    public float BoostAmount = 3.0f;

    private void OnTriggerStay(Collider other)
    {
        other.GetComponentOrThrow(out BallScript ballScript);
        ballScript.Impulse(transform.up * BoostAmount);
    }
}
