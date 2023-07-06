using UnityEngine;
using static UnityEngine.Extensions;

public class NameTagScript : MonoBehaviour
{
    private Transform Cam => Camera.main.transform;
    private Transform parent;

    private void Start()
    {
        this.GetComponentInParentOrThrow(out CheggScript script);
        parent = script.transform;
    }

    private void LateUpdate()
    {
        var angle = Vector3.Angle(Cam.forward, (parent.position - Cam.position).normalized);
        if (angle < 1.0f)
        {
            float Scale = 10.0f * Mathf.Pow(Vector3.Distance(transform.position, Cam.position), 0.5f);
            transform.localScale = new Vector3(Scale, Scale, 0.0f);
            transform.position = parent.position + Vector3.up * (Scale / 30.0f);
            transform.LookAt(Cam.position);
        }
        gameObject.SetActive(angle < 1.0f);
    }
}
