using UnityEngine;

public class WingScript : MonoBehaviour
{
    private Vector3 startRotation;
    private float maxAngle = 45.0f;
    private float currentAngle = 0.0f;
    private float angleStep = 3600.0f;
    private int direction;

    private void Start()
    {
        // wings move opposite each other
        direction = gameObject.name == "Wing" ? 1 : -1;
        startRotation = transform.eulerAngles;
    }

    private void Update()
    {
        currentAngle += direction * angleStep * Time.deltaTime;
        if (currentAngle >= maxAngle)
        {
            direction = -1;
            currentAngle = maxAngle;
        }
        else if (currentAngle <= -maxAngle)
        {
            direction = 1;
            currentAngle = -maxAngle;
        }
        transform.eulerAngles = new Vector3(currentAngle, startRotation.y, startRotation.z);
    }
}