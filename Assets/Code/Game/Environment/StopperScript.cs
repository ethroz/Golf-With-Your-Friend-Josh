using UnityEngine;
using static UnityEngine.Extensions;

public class StopperScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        other.GetComponentOrThrow(out BallScript script);
        script.StopBall();
    }
}
