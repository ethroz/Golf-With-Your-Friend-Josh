using System;
using UnityEngine;
using static UnityEngine.Extensions;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleSpawnManagerScript : SpawnManagerScript
{
    public enum ParticleProperty { MaxParticles, SpawnRate };
    public ParticleProperty Property;

    new private ParticleSystem particleSystem;

    private void Awake()
    {
        this.GetComponentOrThrow(out particleSystem);
    }

    public override void Spawn(int index)
    {
        particleSystem.Stop();
        switch(Property)
        {
            case ParticleProperty.MaxParticles:
                particleSystem.Stop();
                particleSystem.Emit(Settings[index]);
                break;
            case ParticleProperty.SpawnRate:
                var emission = particleSystem.emission;
                emission.rateOverTime = Settings[index] / 100.0f;
                particleSystem.Play();
                break;
            default:
                throw new NotImplementedException();
        }
    }
}
