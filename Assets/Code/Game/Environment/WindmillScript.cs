using UnityEngine;

public class WindmillScript : MonoBehaviour
{
    public float RotationSpeed = 3.0f;
    public Vector3 RotationAxis = Vector3.forward;

    void FixedUpdate()
    {
        transform.RotateAround(transform.position, RotationAxis, RotationSpeed * Time.deltaTime);
    }
}
