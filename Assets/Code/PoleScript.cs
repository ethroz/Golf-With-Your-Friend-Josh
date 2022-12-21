using UnityEngine;

public class PoleScript : MonoBehaviour
{
    public Material NormalMaterial;
    public Material GhostMaterial;
    private Renderer renderr;
    private bool Flash = false;
    private float startTime, currentTime;
    private float frameTime = 1000.0f;
    private float StartRotation;
    private float Range = 1.0f;

    private void Awake()
    {
        renderr = GetComponent<Renderer>();
        NormalMaterial = new Material(NormalMaterial);
        StartRotation = transform.eulerAngles.z;
    }

    public void FlashOn()
    {
        Flash = true;
        startTime = currentTime = Time.time;
        renderr.material = GhostMaterial;
        Invoke("FlashOff", 6.0f);
    }

    public void FlashOff()
    {
        if (IsInvoking())
        {
            CancelInvoke();
        }
        Flash = false;
        renderr.material = NormalMaterial;
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
        if (Flash)
        {
            currentTime += Time.deltaTime;
            float frac = ((currentTime - startTime)) * 1000.0f / frameTime;
            if (frac >= 2.0f)
                startTime = currentTime = Time.time;
            else if (frac < 1.0f)
                GhostMaterial.color = new Color(1.0f, 1.0f, 1.0f, Mathf.Pow(frac, 4.0f));
            else
                GhostMaterial.color = new Color(1.0f, 1.0f, 1.0f, Mathf.Pow(2.0f - frac, 4.0f));
        }
    }
}