using UnityEngine;

public class CheggScript : MonoBehaviour
{
    public Bounds Limit { get; set; }
    private GameObject NameTag;
    private static Transform Eye;
    public float Speed;
    private Vector3 Direction;

    private void Start()
    {
        NameTag = GetComponentInChildren<NameTagScript>().gameObject;
        Eye = Camera.main.GetComponent<Transform>();
        Direction = new Vector3(Random.value, Random.value, Random.value).normalized;
        Speed = Random.value * 1.5f + 0.5f;
    }

    private void FixedUpdate()
    {
        NameTag.SetActive(Vector3.Angle(Eye.forward, (transform.position - Eye.position).normalized) <= 1.0f);
        float Scale = Mathf.Pow(Vector3.Distance(transform.position, Eye.position), 0.4f) / 25.0f;
        transform.localScale = new Vector3(Scale, Scale, Scale);

        if (Time.fixedDeltaTime != 0.0f)
        {
            if (Random.value > 0.9f)
            {
                Direction += 0.5f * (Random.value - 0.5f) * transform.right;
                Direction.Normalize();
            }
            if (Random.value > 0.9f)
            {
                Direction += 0.2f * (Random.value - 0.5f) * transform.up;
                Direction.Normalize();
            }

            if (Physics.Raycast(transform.position, Direction, out RaycastHit rHit, Speed * Time.fixedDeltaTime))
            {
                Direction = rHit.normal;
                transform.position = rHit.point + Direction * (Speed * Time.fixedDeltaTime - rHit.distance);
            }
            else
                transform.position += Direction * Speed * Time.fixedDeltaTime;
            Debug.DrawLine(transform.position, transform.position + Direction * Speed * Time.fixedDeltaTime, Color.red);
            CheckBounds();
            transform.LookAt(transform.position + Direction);
        }
    }

    private void CheckBounds()
    {
        if (transform.position.x < Limit.min.x)
        {
            transform.position = new Vector3(Limit.min.x, transform.position.y, transform.position.z);
            Direction = Vector3.Reflect(Direction, Vector3.right);
        }
        else if (transform.position.x > Limit.max.x)
        {
            transform.position = new Vector3(Limit.max.x, transform.position.y, transform.position.z);
            Direction = Vector3.Reflect(Direction, Vector3.left);
        }
        if (transform.position.y < Limit.min.y)
        {
            transform.position = new Vector3(transform.position.x, Limit.min.y, transform.position.z);
            Direction = Vector3.Reflect(Direction, Vector3.up);
        }
        else if (transform.position.y > Limit.max.y)
        {
            transform.position = new Vector3(transform.position.x, Limit.max.y, transform.position.z);
            Direction = Vector3.Reflect(Direction, Vector3.down);
        }
        if (transform.position.z < Limit.min.z)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, Limit.min.z);
            Direction = Vector3.Reflect(Direction, Vector3.forward);
        }
        else if (transform.position.z > Limit.max.z)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, Limit.max.z);
            Direction = Vector3.Reflect(Direction, Vector3.back);
        }
    }
}