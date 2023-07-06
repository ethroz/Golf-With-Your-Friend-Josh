using UnityEngine;
using static UnityEngine.Extensions;

public class SpeedBoostScript : MonoBehaviour
{
    public float BoostOffset = 10.0f;
    public float BoostMultiplier = 4.0f;

    private void OnTriggerEnter(Collider other)
    {
        other.GetComponentOrThrow(out BallScript script);
        float magnitude = script.GetSpeed();
        script.StopBall();
        script.Impulse(-transform.forward * (BoostMultiplier * magnitude + BoostOffset));
    }
}
