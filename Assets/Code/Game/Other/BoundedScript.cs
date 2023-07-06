using UnityEngine;

public class BoundedScript : MonoBehaviour
{
    public Bounds Bounds { get; private set; }

    public void SetBounds(Bounds bounds)
    {
        Bounds = bounds;
    }
}
