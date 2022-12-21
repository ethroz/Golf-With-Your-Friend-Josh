using System.Collections.Generic;
using UnityEngine;

public class LineRendererScript : MonoBehaviour
{
    public GameObject Ray;
    private Transform Eye;
    private GameObject[] Rays;
    private List<Vector3> Verts;
    public Vector3[] Vertices => Verts.ToArray();
    public int VertexCount => Verts.Count;
    private bool bShow = false;

    private void Start()
    {
        Eye = Camera.main.transform;
        Verts = new List<Vector3>();
    }

    private void LateUpdate()
    {
        if (VertexCount < 2 || !bShow)
            return;
        for (int i = 0; i < Rays.Length; i++)
        {
            Vector3 ray = (Verts[i + 1] - Verts[i]).normalized;
            Vector3 rayRight = Vector3.Cross(Vector3.up, ray);
            Vector3 rayUp = Vector3.Cross(ray, rayRight);
            float t = Vector3.Dot(ray, Eye.position - Verts[i]);
            Vector3 RayToEye = Eye.position - t * ray - Verts[i];
            float roll = Vector3.Angle(rayUp, RayToEye);
            if (Vector3.Angle(rayRight, RayToEye) < 90.0f)
                roll *= -1.0f;
            Rays[i].transform.eulerAngles = new Vector3(Rays[i].transform.eulerAngles.x, Rays[i].transform.eulerAngles.y, roll);
        }
    }

    public void Show()
    {
        bShow = true;
        if (VertexCount < 2)
            return;
        for (int i = 0; i < Rays.Length; i++)
            Rays[i].SetActive(true);
    }

    public void Hide()
    {
        bShow = false;
        if (VertexCount < 2)
            return;
        for (int i = 0; i < Rays.Length; i++)
            Rays[i].SetActive(false);
    }

    public void Create()
    {
        if (VertexCount < 2)
            return;
        Delete();
        Rays = new GameObject[VertexCount - 1];
        for (int i = 0; i < Rays.Length; i++)
        {
            Rays[i] = Instantiate(Ray);
            Rays[i].transform.position = (Verts[i] + Verts[i + 1]) / 2.0f;
            Vector3 ray = Verts[i + 1] - Verts[i];
            float yaw = Vector3.Angle(Vector3.forward, new Vector3(ray.x, 0.0f, ray.z));
            if (ray.x < 0.0f)
                yaw *= -1.0f;
            float pitch = -Vector3.Angle(ray, new Vector3(ray.x, 0.0f, ray.z));
            Rays[i].transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
            Rays[i].transform.localScale = new Vector3(Rays[i].transform.localScale.x, 1.0f, Rays[i].transform.localScale.z * ray.magnitude);
            Rays[i].SetActive(bShow);
        }
    }

    public void Delete()
    {
        if (VertexCount < 2 || Rays == null)
            return;
        for (int i = 0; i < Rays.Length; i++)
            Destroy(Rays[i]);
        Rays = null;
    }

    public void Set(Vector3[] vs)
    {
        Verts = new List<Vector3>();
        Verts.AddRange(vs);
    }

    public void Clear()
    {
        Verts = new List<Vector3>();
    }

    public void Add(Vector3 v)
    {
        Verts.Add(v);
    }

    public void AddRange(Vector3[] vs)
    {
        Verts.AddRange(vs);
    }
}