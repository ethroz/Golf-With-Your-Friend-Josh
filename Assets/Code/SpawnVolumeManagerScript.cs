using UnityEngine;

public class SpawnVolumeManagerScript : MonoBehaviour
{
    public GameObject Prefab;
    public int NumberOfObjects = 50;
    private Bounds Barrier;
    private GameObject[] spawns;

    private void Start()
    {
        Transform[] ts = GetComponentsInChildren<Transform>();
        foreach (Transform t in ts)
        {
            if (t.name == "Min Bounds")
                Barrier.min = t.position;
            else if (t.name == "Max Bounds")
                Barrier.max = t.position;
        }
        if (Barrier.min == Vector3.zero || Barrier.max == Vector3.zero)
            Debug.LogError("no bounds");
        Spawn();
    }

    public void Spawn()
    {
        if (Prefab != null)
        {
            if (spawns != null)
                for (int i = 0; i < spawns.Length; i++)
                    Destroy(spawns[i]);
            spawns = new GameObject[NumberOfObjects];
            for (int i = 0; i < NumberOfObjects; i++)
            {
                Vector3 randomPos = new Vector3(Random.Range(Barrier.min.x, Barrier.max.x), Random.Range(Barrier.min.y, Barrier.max.y), Random.Range(Barrier.min.z, Barrier.max.z));
                spawns[i] = Instantiate(Prefab, randomPos, new Quaternion(), transform);
                spawns[i].GetComponentInChildren<CheggScript>().Limit = Barrier;
            }
        }
    }
}