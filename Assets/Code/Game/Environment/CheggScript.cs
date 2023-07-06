using UnityEngine;

public class CheggScript : BoundedScript
{
    public float Speed { get; private set; }
    private Transform Cam => Camera.main.transform;
    private Vector3 direction;

    private void Start()
    {
        direction = new Vector3(Random.value, Random.value, Random.value).normalized;
        Speed = Random.value * 1.5f + 0.5f;
    }

    private void FixedUpdate()
    {
        var Scale = Mathf.Pow(Vector3.Distance(transform.position, Cam.position), 0.4f) / 25.0f;
        transform.localScale = new Vector3(Scale, Scale, Scale);

        if (Time.fixedDeltaTime != 0.0f)
        {
            if (Random.value > 0.9f)
            {
                direction += 0.5f * (Random.value - 0.5f) * transform.right;
                direction.Normalize();
            }
            if (Random.value > 0.9f)
            {
                direction += 0.2f * (Random.value - 0.5f) * transform.up;
                direction.Normalize();
            }

            if (Physics.Raycast(transform.position, direction, out RaycastHit rHit, Speed * Time.fixedDeltaTime))
            {
                direction = rHit.normal;
                transform.position = rHit.point + direction * (Speed * Time.fixedDeltaTime - rHit.distance);
            }
            else
                transform.position += direction * Speed * Time.fixedDeltaTime;
            Debug.DrawLine(transform.position, transform.position + direction * Speed * Time.fixedDeltaTime, Color.red);
            CheckBounds();
            transform.LookAt(transform.position + direction);
        }
    }

    private void CheckBounds()
    {
        if (transform.position.x < Bounds.min.x)
        {
            transform.position = new Vector3(Bounds.min.x, transform.position.y, transform.position.z);
            direction = Vector3.Reflect(direction, Vector3.right);
        }
        else if (transform.position.x > Bounds.max.x)
        {
            transform.position = new Vector3(Bounds.max.x, transform.position.y, transform.position.z);
            direction = Vector3.Reflect(direction, Vector3.left);
        }
        if (transform.position.y < Bounds.min.y)
        {
            transform.position = new Vector3(transform.position.x, Bounds.min.y, transform.position.z);
            direction = Vector3.Reflect(direction, Vector3.up);
        }
        else if (transform.position.y > Bounds.max.y)
        {
            transform.position = new Vector3(transform.position.x, Bounds.max.y, transform.position.z);
            direction = Vector3.Reflect(direction, Vector3.down);
        }
        if (transform.position.z < Bounds.min.z)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, Bounds.min.z);
            direction = Vector3.Reflect(direction, Vector3.forward);
        }
        else if (transform.position.z > Bounds.max.z)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, Bounds.max.z);
            direction = Vector3.Reflect(direction, Vector3.back);
        }
    }
}
