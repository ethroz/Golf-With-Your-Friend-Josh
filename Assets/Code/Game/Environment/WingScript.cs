using UnityEngine;

public class WingScript : MonoBehaviour
{
    public float MaxAngle = 45.0f;
    public float DegreesPerSecond = 3600.0f;
    public Vector3 RotationAxis = Vector3.forward;

    private Quaternion startRotation;
    private float currentAngle;
    private int direction;

    private void Start()
    {
        // wings move opposite each other
        direction = gameObject.name.Contains("(1)") ? 1 : -1;
        startRotation = transform.localRotation;
        currentAngle = 0.0f;
    }

    private void Update()
    {
        float deltaAngle = DegreesPerSecond * Time.deltaTime;
        while (Mathf.Abs(currentAngle + direction * deltaAngle) > MaxAngle)
        {
            deltaAngle -= MaxAngle - direction * currentAngle;
            currentAngle = direction * MaxAngle;
            direction *= -1;
        }
        currentAngle += deltaAngle * direction;
        transform.localRotation = startRotation;
        transform.Rotate(RotationAxis, currentAngle);
    }
}
