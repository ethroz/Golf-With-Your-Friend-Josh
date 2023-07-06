using UnityEngine;
using static UnityEngine.Extensions;

public class BoundedSpawnManagerScript : SpawnManagerScript
{
    public GameObject Prefab;

    private Bounds Barrier;
    private GameObject[] spawns;

    private void Awake()
    {
        ThrowIfNull(Prefab);
        ThrowIfNull(Settings);
        Prefab.AssertHasComponent<BoundedScript>();

        Transform[] ts = GetComponentsInChildren<Transform>();
        foreach (Transform t in ts)
        {
            if (t.name == "Min Bounds")
                Barrier.min = t.position;
            else if (t.name == "Max Bounds")
                Barrier.max = t.position;
        }
        if (Barrier.min == Vector3.zero || Barrier.max == Vector3.zero)
        {
            throw new MissingComponentException(nameof(Bounds));
        }
    }

    public override void Spawn(int index)
    {
        if (spawns != null)
        {
            for (int i = 0; i < spawns.Length; ++i)
            {
                Destroy(spawns[i]);
            }
        }
        int num = Settings[index];
        spawns = new GameObject[num];
        for (int i = 0; i < num; ++i)
        {
            Vector3 randomPos = new Vector3(Random.Range(Barrier.min.x, Barrier.max.x), Random.Range(Barrier.min.y, Barrier.max.y), Random.Range(Barrier.min.z, Barrier.max.z));
            spawns[i] = Instantiate(Prefab, randomPos, new Quaternion(), transform);
            var script = spawns[i].GetComponent<BoundedScript>();
            script.SetBounds(Barrier);
        }
    }
}
