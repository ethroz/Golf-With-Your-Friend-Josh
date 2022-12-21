using UnityEngine;

public class TeleporterScript : MonoBehaviour
{
    public float ShootSpeed = 8.0f;
    public float TunnelTime = 2.0f;
    private Transform exporter;

    void Start()
    {
        exporter = GetComponentInChildren<ExporterTag>().GetComponent<Transform>();
    }

    private void ShootBall()
    {
        GameManagerScript.CurrentBall.SetActive(true);
        GameManagerScript.CurrentPlayer.BallScript.Impulse(exporter.forward * ShootSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        GameManagerScript.CurrentPlayer.BallScript.StopBall();
        GameManagerScript.CurrentPlayer.BallScript.MoveBall(exporter.position);
        GameManagerScript.CurrentBall.SetActive(false);
        Invoke("ShootBall", TunnelTime);
    }
}