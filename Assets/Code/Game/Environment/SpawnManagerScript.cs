using UnityEngine;

public abstract class SpawnManagerScript : MonoBehaviour
{
    public int[] Settings;

    public abstract void Spawn(int index);
}
