using UnityEngine;

public class WaterScript : MonoBehaviour
{
    public ParticleSystem SplashFX;
    public const float DENSITY_OF_WATER = 997.0f;
    private float otherDensity;
    private ParticleSystem splashInstance;

    private void OnTriggerEnter(Collider other)
    {
        other.GetComponentOrThrow(out BallScript script);
        otherDensity = script.FluidDensity;
        script.FluidDensity = DENSITY_OF_WATER;
        splashInstance = Instantiate(SplashFX);
        splashInstance.transform.position = other.transform.position;
        splashInstance.Play();
        script.InWater = true;
        script.Splash = other.transform.position;
    }

    private void OnTriggerExit(Collider other)
    {
        other.GetComponentOrThrow(out BallScript script);
        script.FluidDensity = otherDensity;
        script.InWater = false;
        Destroy(splashInstance);
    }
}
