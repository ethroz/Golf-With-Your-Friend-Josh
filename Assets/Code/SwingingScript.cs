using UnityEngine;

public class SwingingScript : MonoBehaviour
{
    public float MaxAngle = 45.0f;
    public int frameTime = 200;
    private float startTime;
    private float[] keyFrames;
    private Quaternion startRot;

    private void Start()
    {
        startTime = Time.time;
        keyFrames = new float[] { -1.0f, -0.95f, -0.6f, 0.0f, 0.6f, 0.95f, 1.0f, 0.95f, 0.6f, 0.0f, -0.6f, -0.95f };
    }

    private void LateUpdate()
    {
        int index = Mathf.FloorToInt((Time.time - startTime) * 1000.0f / frameTime) % keyFrames.Length;
        Vector3 k1 = new Vector3(keyFrames[index] * MaxAngle, 0, 0);
        Vector3 k2 = new Vector3(keyFrames[(index + 1) % keyFrames.Length] * MaxAngle, 0, 0);
        float frac = ((Time.time - startTime) * 1000.0f / frameTime - index) % keyFrames.Length;
        float angle = Vector3.Lerp(k1, k2, frac).x;
        transform.eulerAngles = new Vector3();
        transform.RotateAround(transform.position, transform.right, angle);
    }
}