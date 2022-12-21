using UnityEngine;

public class WindmillBladePushScript : MonoBehaviour
{
    private float BoostAmount;
    private Vector3 direction;

    void Start()
    {
        WindmillScript ws = GetComponentInParent<WindmillScript>();
        direction = GetComponentInParent<WindmillTag>().transform.right * Mathf.Abs(ws.RotationSpeed);
        BoostAmount = ws.RotationSpeed * Mathf.Deg2Rad * transform.localScale.y / 2.0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<BallScript>().Impulse(direction * -BoostAmount);
    }
}