using UnityEngine;

public class PoleRadiusScript : MonoBehaviour
{
    private GameObject Pole;
    private float Direction = 0.0f;

    void Start()
    {
        Pole = GetComponentInChildren<PoleScript>().gameObject;
    }

    private void Update()
    {
        if (Direction != 0.0f)
            Pole.transform.localPosition += Direction * Vector3.up * Time.deltaTime;
        if (Pole.transform.localPosition.y >= 1.0f)
        {
            Pole.transform.localPosition = new Vector3(Pole.transform.localPosition.x, 1.0f, Pole.transform.localPosition.z);
            Direction = 0.0f;
        }
        if (Pole.transform.position.y <= 0.0f)
        {
            Pole.transform.localPosition = new Vector3(Pole.transform.localPosition.x, 0.0f, Pole.transform.localPosition.z);
            Direction = 0.0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Direction = 5.0f;
    }

    private void OnTriggerExit(Collider other)
    {
        Direction = -5.0f;
    }
}