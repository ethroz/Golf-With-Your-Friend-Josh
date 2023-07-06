using UnityEngine;
using static UnityEngine.Extensions;

public class BladeScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        other.GetComponentOrThrow(out BallScript ballScript);
        ballScript.Dead = true;
    }
}
