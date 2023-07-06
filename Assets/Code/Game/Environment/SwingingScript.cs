using UnityEngine;

public class SwingingScript : MonoBehaviour
{
    public float MaxAngle = 45.0f;
    public int FrameTime = 200;
    public Vector3 RotationAxis = Vector3.right;

    private float startTime;
    private Quaternion startRot;
    private float[] keyFrames = new float[] { -1.0f, -0.95f, -0.6f, 0.0f, 0.6f, 0.95f, 1.0f, 0.95f, 0.6f, 0.0f, -0.6f, -0.95f };

    private void Start()
    {
        startTime = Time.time;
        startRot = transform.rotation;
    }

    private void LateUpdate()
    {
        var time = (Time.time - startTime) * 1000.0f / FrameTime;
        var index = Mathf.FloorToInt(time % keyFrames.Length);
        var k1 = keyFrames[index] * MaxAngle;
        var k2 = keyFrames[(index + 1) % keyFrames.Length] * MaxAngle;
        var frac = (time - index) % keyFrames.Length;
        var angle = Mathf.Lerp(k1, k2, frac);
        transform.rotation = startRot;
        transform.RotateAround(transform.position, RotationAxis, angle);
    }
}
