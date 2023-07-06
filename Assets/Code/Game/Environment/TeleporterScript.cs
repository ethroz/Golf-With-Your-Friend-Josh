using UnityEngine;

public class TeleporterScript : MonoBehaviour
{
    public float ShootSpeed = 8.0f;
    public float TunnelTime = 2.0f;
    private Transform exporter;
    private GameObject ball;
    private BallScript ballScript;

    private void Start()
    {
        this.GetComponentInChildrenOrThrow(out ExporterTag exporter);
        this.exporter = exporter.transform;
    }

    private void ShootBall()
    {
        ball.SetActive(true);
        ballScript.Impulse(exporter.forward * ShootSpeed);
        ball = null;
        ballScript = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        ball = other.gameObject;
        ball.SetActive(false);
        other.GetComponentOrThrow(out ballScript);
        ballScript.StopBall();
        ballScript.MoveBall(exporter.position);
        Invoke(nameof(ShootBall), TunnelTime);
    }
}
