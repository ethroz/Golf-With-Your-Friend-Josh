using UnityEngine;
using static UnityEngine.Extensions;

[RequireComponent(typeof(Renderer))]
public class PoleScript : MonoBehaviour
{
    public Material NormalMaterial;
    public Material GhostMaterial;

    new private Renderer renderer;
    private bool flash = false;
    private float startTime;
    private float frameTime = 1000.0f;
    private float StartRotation;
    private float Range = 1.0f;

    private void Awake()
    {
        this.GetComponentOrThrow(out renderer);
        NormalMaterial = new Material(NormalMaterial);
        StartRotation = transform.eulerAngles.z;
    }

    private void FixedUpdate()
    {
        // wind
        transform.eulerAngles += new Vector3(0.0f, 0.0f, (Random.value - 0.5f));
        if (transform.eulerAngles.z > StartRotation + Range)
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, StartRotation + Range);
        if (transform.eulerAngles.z < StartRotation - Range)
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, StartRotation - Range);
    }

    private void LateUpdate()
    {
        if (flash)
        {
            float frac = ((Time.time - startTime)) * 1000.0f / frameTime;
            if (frac >= 2.0f)
                startTime = Time.time;
            else if (frac < 1.0f)
                GhostMaterial.color = new Color(1.0f, 1.0f, 1.0f, Mathf.Pow(frac, 4.0f));
            else
                GhostMaterial.color = new Color(1.0f, 1.0f, 1.0f, Mathf.Pow(2.0f - frac, 4.0f));
        }
    }

    public void FlashOn()
    {
        flash = true;
        startTime = Time.time;
        renderer.material = GhostMaterial;
        Invoke(nameof(FlashOff), 6.0f);
    }

    public void FlashOff()
    {
        if (IsInvoking())
            CancelInvoke();
        flash = false;
        renderer.material = NormalMaterial;
    }
}
