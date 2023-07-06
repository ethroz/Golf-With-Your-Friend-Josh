using UnityEngine;
using static UnityEngine.Extensions;

public class WindmillBladePushScript : MonoBehaviour
{
    private WindmillScript parentScript;

    void Start()
    {
        this.GetComponentInParentOrThrow(out parentScript);
    }

    private void OnTriggerEnter(Collider other)
    {
        other.GetComponentOrThrow(out BallScript script);
        Vector3 toBall = other.transform.position - transform.position;
        Vector3 hitDir = Vector3.Cross(toBall, parentScript.RotationAxis).normalized;
        float hitSpeed = Mathf.Abs(parentScript.RotationSpeed) * Mathf.Deg2Rad * toBall.magnitude;
        script.Impulse(hitSpeed * hitDir);
        print(hitDir);
    }
}
