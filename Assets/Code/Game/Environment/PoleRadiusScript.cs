using UnityEngine;
using static UnityEngine.Extensions;

public class PoleRadiusScript : MonoBehaviour
{
    public float Magnitude = 5.0f;

    private GameObject pole;
    private int direction = 0;

    void Start()
    {
        this.GetComponentInChildrenOrThrow(out PoleScript script);
        pole = script.gameObject;
    }

    private void Update()
    {
        if (direction != 0)
            pole.transform.localPosition += direction * Magnitude * Vector3.up * Time.deltaTime;
        if (pole.transform.localPosition.y >= 1.0f)
        {
            pole.transform.localPosition = new Vector3(pole.transform.localPosition.x, 1.0f, pole.transform.localPosition.z);
            direction = 0;
        }
        if (pole.transform.position.y <= 0.0f)
        {
            pole.transform.localPosition = new Vector3(pole.transform.localPosition.x, 0.0f, pole.transform.localPosition.z);
            direction = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        direction = 1;
    }

    private void OnTriggerExit(Collider other)
    {
        direction = -1;
    }
}
