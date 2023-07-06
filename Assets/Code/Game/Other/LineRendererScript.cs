using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Extensions;

public class LineRendererScript : MonoBehaviour
{
    public GameObject Ray;

    private Transform Cam => Camera.main.transform;
    private List<GameObject> rays;
    private List<Vector3> vertices;
    private bool show = false;

    private void Awake()
    {
        ThrowIfNull(Ray);
        Clear();
    }

    private void LateUpdate()
    {
        if (!show)
            return;
        for (int i = 0; i < rays.Count; ++i)
        {
            Vector3 ray = (vertices[i + 1] - vertices[i]).normalized;
            Vector3 rayRight = Vector3.Cross(Vector3.up, ray);
            Vector3 rayUp = Vector3.Cross(ray, rayRight);
            float t = Vector3.Dot(ray, Cam.position - vertices[i]);
            Vector3 RayToEye = Cam.position - t * ray - vertices[i];
            float roll = Vector3.Angle(rayUp, RayToEye);
            if (Vector3.Angle(rayRight, RayToEye) < 90.0f)
                roll *= -1.0f;
            rays[i].transform.eulerAngles = new(rays[i].transform.eulerAngles.x, rays[i].transform.eulerAngles.y, roll);
        }
    }

    public void Show(bool show)
    {
        if (this.show != show)
        {
            gameObject.SetActive(show);
        }
        this.show = show;
    }

    public void Set(Vector3[] vs)
    {
        vertices = new(vs);
        UpdateRays();
    }

    public void Clear()
    {
        vertices = new();
        rays = new();
    }

    public void Add(Vector3 v)
    {
        vertices.Add(v);
        UpdateRays();
    }

    public void AddRange(Vector3[] vs)
    {
        vertices.AddRange(vs);
        UpdateRays();
    }

    private void UpdateRays()
    {
        int i = 0;
        for (; i < vertices.Count - 1; ++i)
        {
            if (i >= rays.Count)
            {
                rays.Add(Instantiate(Ray, transform));
            }

            rays[i].transform.position = (vertices[i] + vertices[i + 1]) / 2.0f;
            Vector3 ray = vertices[i + 1] - vertices[i];
            float yaw = Vector3.Angle(Vector3.forward, new(ray.x, 0.0f, ray.z));
            if (ray.x < 0.0f)
                yaw *= -1.0f;
            float pitch = -Vector3.Angle(ray, new(ray.x, 0.0f, ray.z));
            rays[i].transform.eulerAngles = new(pitch, yaw, 0.0f);
            rays[i].transform.localScale = new(1.0f, 1.0f, ray.magnitude);
        }
        for (int j = i; j < rays.Count; ++j)
        {
            Destroy(rays[j]);
        }
        rays.RemoveRange(i, rays.Count - i);
    }
}
