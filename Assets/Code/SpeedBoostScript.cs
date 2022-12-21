using UnityEngine;

public class SpeedBoostScript : MonoBehaviour
{
    public float MinBoostAmount = 10.0f;
    public float MaxBoostAmount = 40.0f;

    private void OnTriggerEnter(Collider other)
    {
        float magnitude = GameManagerScript.CurrentPlayer.BallScript.GetVeloMag();
        GameManagerScript.CurrentPlayer.BallScript.StopBall();
        GameManagerScript.CurrentPlayer.BallScript.Impulse(-transform.forward * Map(magnitude, 0.0f, GameManagerScript.CurrentPlayer.ClubScript.MaxPower, MinBoostAmount, MaxBoostAmount));
    }

    public static float Map(float value, float oldMin, float oldMax, float newMin, float newMax)
    {
        return (value - oldMin) / (oldMax - oldMin) * (newMax - newMin) + newMin;
    }
}