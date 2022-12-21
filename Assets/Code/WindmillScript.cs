using UnityEngine;

public class WindmillScript : MonoBehaviour
{
    public float RotationSpeed { get; private set; } = 3.0f;

    void FixedUpdate()
    {
        transform.Rotate(new Vector3(0.0f, 0.0f, RotationSpeed));
    }
}