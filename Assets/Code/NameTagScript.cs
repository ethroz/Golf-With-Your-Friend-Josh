using UnityEngine;

public class NameTagScript : MonoBehaviour
{
    private static Transform Eye;
    private Transform Parent;
    private static float VerticalOffset;

    private void Start()
    {
        Eye = Camera.main.transform;
        Parent = GetComponentInParent<CheggScript>().gameObject.transform;
        VerticalOffset = transform.position.y / transform.localScale.y;
    }

    private void LateUpdate()
    {
        float Scale = 10.0f * Mathf.Pow(Vector3.Distance(transform.position, Eye.position), 0.5f);
        transform.localScale = new Vector3(Scale, Scale, 0.0f);
        transform.position = Parent.position + Vector3.up * (Scale / 30.0f);
        transform.LookAt(Eye.position);
    }
}