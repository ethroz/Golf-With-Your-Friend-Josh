using UnityEngine;

public class WaterScript : MonoBehaviour
{
    public ParticleSystem psSplash;
    private float fDefaultDensity;
    public const float fDenity = 997.0f;

    private void OnTriggerEnter(Collider other)
    {
        fDefaultDensity = GameManagerScript.CurrentPlayer.BallScript.FluidDensity;
        GameManagerScript.CurrentPlayer.BallScript.FluidDensity = fDenity;
        ParticleSystem instance = Instantiate(psSplash);
        instance.transform.position = other.transform.position;
        instance.Play();
        GameManagerScript.CurrentPlayer.BallScript.InWater = true;
        GameManagerScript.CurrentPlayer.BallScript.Splash = other.transform.position;
    }

    private void OnTriggerExit(Collider other)
    {
        GameManagerScript.CurrentPlayer.BallScript.FluidDensity = fDefaultDensity;
        GameManagerScript.CurrentPlayer.BallScript.InWater = false;
    }
}