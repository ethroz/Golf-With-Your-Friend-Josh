using UnityEngine;

public class ClubScript : MonoBehaviour
{
    public Player Owner { get; set; }
    public float MaxPower = 30.0f;
    public float WindUpTime { get; private set; }

    private bool hit;
    private float shotPower;
    private float angle;
    private bool swinging;
    private float[] keyFrames = new float[] { 0.0f, -120.0f, -135.0f, 0.0f, 90.0f, 80.0f, 0.0f };
    private Quaternion startRot;
    private float lastAngle;
    private float frameTime = 0.2f;
    private float startTime;

    private void Awake()
    {
        WindUpTime = frameTime * (keyFrames.Length - 1) / 2;
    }

    private void Update()
    {
        if (swinging)
        {
            float frac = shotPower / MaxPower;
            float time = (Time.time - startTime);
            int index = (int)(time / frameTime);
            if (index == keyFrames.Length - 1)
            {
                transform.rotation = startRot;
                swinging = false;
                return;
            }
            if (time >= WindUpTime && !hit)
            {
                hit = true;
                HitBall();
            }
            Vector3 k1 = Vector3.right * keyFrames[index] * frac;
            Vector3 k2 = Vector3.right * keyFrames[index + 1] * frac;
            float angle = Vector3.Slerp(k1, k2, time / frameTime - index).x;
            transform.RotateAround(transform.position, transform.right, lastAngle - angle);
            lastAngle = angle;
        }
    }

    public void SwingClub(float power, float angle)
    {
        shotPower = power;
        this.angle = angle;
        startRot = transform.rotation;
        lastAngle = 0.0f;
        swinging = true;
        startTime = Time.time;
    }

    private void HitBall()
    {
        Owner.BallScript.Impulse(shotPower * new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0.0f, Mathf.Cos(angle * Mathf.Deg2Rad)));
        Owner.BallScript.NotMoving = false;
    }
}