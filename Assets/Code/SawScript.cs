using UnityEngine;

public class SawScript : MonoBehaviour
{
    public float Speed = 300.0f;

    private void LateUpdate()
    {
        transform.Rotate(new Vector3(0.0f, 0.0f, Speed * Time.deltaTime));
    }
}