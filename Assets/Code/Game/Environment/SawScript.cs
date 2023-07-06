using UnityEngine;

public class SawScript : MonoBehaviour
{
    public float Speed = 300.0f;
    public Vector3 RotationAxis = Vector3.forward;

    private void LateUpdate()
    {
        transform.Rotate(Speed * Time.deltaTime * RotationAxis);
    }
}
