using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using static UnityEngine.Extensions;

[RequireComponent(typeof(TrailRenderer))]
public class BallScript : MonoBehaviour, IGameListener
{
    public GameManagerScript Game { get; private set; }
    public float Radius { get; private set; }
    public bool IsMoving { get; private set; }
    public bool Dead { get; set; }
    public bool Respawn { get; private set; }
    public bool InWater { get; set; }
    public Vector3 Splash { get; set; }
    public float FluidDensity { get; set; }
    public const int LayerMask = ~((1 << 4) + (1 << 2));
    private static Vector3 Gravity = new Vector3(0.0f, -9.81f, 0.0f);

    private TrailRenderer trail;
    private Stopwatch timer;
    private const long forceStopInterval = 500;
    private Vector3 velocity;
    private Vector3 position
    {
        get => transform.position;
        set => transform.position = value;
    }
    private float angularSpeed;
    private const int NumberOfRays = 3;
    private RaycastHit rHit;
    private float Distance;
    private float Time;
    private Vector3 offset;
    private Vector3 groundNormal;
    private const float fShallowAngle = 10.0f;
    private const float SkinOffset = 0.99f;
    private const float fGroundOffset = 0.001f;
    private const float rollingResistanceCoefficient = 0.2f;
    private const float dragCoefficient = 0.47f;
    private float CSArea;

    private void Awake()
    {
        this.GetComponentOrThrow(out trail);
        Radius = transform.localScale.y / 2.0f;
        FluidDensity = 1.225f;
        InWater = false;
        Dead = false;
        CSArea = Mathf.PI * Radius * Radius;
        timer = new Stopwatch();
        groundNormal = -Gravity.normalized;
        velocity = new Vector3();
    }

    public void SetManager(GameManagerScript manager)
    {
        Game = manager;
    }

    public void Impulse(Vector3 vec)
    {
        velocity += vec;
        IsMoving = velocity != Vector3.zero;
    }

    public void MoveBall(Vector3 position)
    {
        this.position = position;
        trail.Clear();
    }

    public void StopBall()
    {
        velocity = new Vector3();
    }

    public void KillBall()
    {
        StopBall();
        Dead = true;
    }

    public float GetSpeed()
    {
        return velocity.magnitude;
    }

    private bool Grounded()
    {
        if (Physics.Raycast(position, -groundNormal, out rHit, Radius + fGroundOffset, LayerMask) && rHit.normal == groundNormal)
        {
            position = rHit.point + groundNormal * Radius;

            //print("On the ground");
            return true;
        }
        //else print("Not on the ground");
        return false;
    }

    private bool WillCollide()
    {
        Vector3 vDir = velocity.normalized;
        List<RaycastHit> rHits = new List<RaycastHit>();
        List<Vector3> offsets = new List<Vector3>();
        Vector3 horizontalPerpendicular = Vector3.Cross(vDir, Vector3.up).normalized * Radius;
        if (horizontalPerpendicular.sqrMagnitude == 0.0f)
            horizontalPerpendicular = Vector3.right;
        for (int i = 0; i < NumberOfRays; ++i)
        {
            int single = NumberOfRays - i;
            Vector3 horizontal = (horizontalPerpendicular * (single - 1) / (NumberOfRays - 1)) * SkinOffset;
            Vector3 forwards = vDir * Mathf.Sqrt(Radius * Radius - horizontal.magnitude * horizontal.magnitude);
            Quaternion rotation = Quaternion.AngleAxis(360.0f / single / single, vDir);
            for (int j = 0; j < single * single; ++j)
            {
                horizontal = rotation * horizontal;
                Vector3 offset = horizontal + forwards;
                if (Physics.Raycast(position + offset, vDir, out RaycastHit rHit, Distance, LayerMask))
                {
                    rHits.Add(rHit);
                    offsets.Add(offset);
                    //Debug.DrawLine(vPosition + offset, vPosition + offset + vDir * rHit.distance, Color.black);
                }
                //else Debug.DrawLine(vPosition + offset, vPosition + offset + vVelocity * fTime, Color.black);
            }
        }
        if (rHits.Count == 0)
        {
            //print("will not collide");
            return false;
        }

        int bestDistanceIndex = 0;
        for (int i = 1; i < rHits.Count; ++i)
        {
            if (rHits[i].distance < rHits[bestDistanceIndex].distance)
                bestDistanceIndex = i;
        }
        rHit = rHits[bestDistanceIndex];
        offset = offsets[bestDistanceIndex];
        //print("will collide");
        return true;
    }

    private void FixedUpdate()
    {
        IsMoving = true;
        Respawn = false;

        // air resistance
        if (velocity.sqrMagnitude != 0.0f)
            velocity -= 0.5f * CSArea * dragCoefficient * FluidDensity * UnityEngine.Time.fixedDeltaTime * velocity.sqrMagnitude * velocity.normalized;

        if (Grounded()) // if the ball is on the same sloped ground as last frame
        {
            // if the ball is on the terrain or on the wrong level or dead then respawn
            if (rHit.collider.gameObject.CompareTag("Terrain") || Dead)
            {
                Respawn = true;
            }
            else
            {
                var levelScript = rHit.collider.GetComponentInParent<LevelScript>();
                if (levelScript != null && levelScript.LevelIndex != Game.LevelIndex)
                {
                    Respawn = true;
                }
            }

            // rotational resistance
            float gravityAngle = Vector3.Angle(-groundNormal, Gravity) * Mathf.Deg2Rad; // get the angle between the ground and gravity
            velocity -= rollingResistanceCoefficient * Gravity.magnitude * Mathf.Cos(gravityAngle) * velocity.normalized * UnityEngine.Time.fixedDeltaTime//;
                * (velocity.sqrMagnitude / 100.0f + 1.0f); // apply rotational resistance dependent on speed squared to slow down the ball at fast speeds

            // if ball is not moving fast enough for a certain amount of time then stop it
            if (velocity.magnitude < 0.1f)
            {
                if (!timer.IsRunning)
                    timer.Restart();
                if (timer.ElapsedMilliseconds > forceStopInterval)
                {
                    velocity = new Vector3();
                    IsMoving = false;
                    return;
                }
            }
            else
            {
                timer.Reset();
            }

            Vector3 previousVelocity = velocity;
            Vector3 gravityDir = new Vector3();
            if (groundNormal != -Gravity.normalized)
            {
                gravityDir = Vector3.Cross(groundNormal, Vector3.Cross(Gravity, groundNormal)).normalized; // the direction gravity pushes the ball on the surface
                velocity += gravityDir * (Mathf.Sin(gravityAngle) * Gravity.magnitude * UnityEngine.Time.fixedDeltaTime); // gravity pushes the ball down a hill
            }

            Time = UnityEngine.Time.fixedDeltaTime;
            Distance = velocity.magnitude * Time;
            float fDistanceTraveled = 0.0f;
            while (Time > 0.0f) // collision loop
            {
                if (WillCollide()) // if the ball is going to collide with something
                {
                    Vector3 prevPos = position;
                    position = rHit.point - offset; // move the ball to the offset collision point
                    if (prevPos == position) // avoid getting stuck in the loop
                        break;
                    trail.AddPosition(position); // add the new position to the trail renderer
                    float d0 = (position - prevPos).magnitude;
                    fDistanceTraveled += d0;
                    Distance -= d0; // update the distance traveled by the ball
                    float angle = Vector3.Angle(rHit.normal, groundNormal); // angle between the old ground normal and the new normal
                    if (angle < fShallowAngle) // if the next collision is at a shallow enough angle then assume it is rounded ground
                    {
                        float t0;
                        // t0 depends on if there is a force or not
                        if (gravityAngle == 0.0f)
                            t0 = d0 / velocity.magnitude;
                        else
                        {
                            t0 = (-previousVelocity.magnitude + Mathf.Sqrt(previousVelocity.sqrMagnitude + 2.0f * Mathf.Sin(gravityAngle) *
                                Gravity.magnitude * d0)) / (Mathf.Sin(gravityAngle) * Gravity.magnitude); // time taken to go from starting position to intersection position
                        }
                        Time -= t0;
                        previousVelocity = velocity = velocity - gravityDir * (Mathf.Sin(gravityAngle) * Gravity.magnitude * Time); // new velocity without gravity outside of intersection
                        groundNormal = rHit.normal; // set the ground normal to the collision normal
                        gravityAngle = Vector3.Angle(-groundNormal, Gravity) * Mathf.Deg2Rad; // get the angle between the ground and gravity
                        gravityDir = Vector3.Cross(groundNormal, Vector3.Cross(Gravity, groundNormal)).normalized; // the direction gravity pushes the ball on the surface
                        Vector3 newVelocityDirection = Vector3.Cross(Vector3.Cross(-velocity, groundNormal), groundNormal).normalized; // the projection of the velocity onto a new surface
                        velocity = newVelocityDirection * (velocity.magnitude * Mathf.Cos(angle * Mathf.Deg2Rad))
                            + gravityDir * (Mathf.Sin(gravityAngle) * Gravity.magnitude * Time); // the new velocity on the new surface of any slope
                        Distance = velocity.magnitude * Time; // calculate the new remaining distance before end of frame time
                        //print(UnityEngine.Time.frameCount + " new slope: " + gravityAngle * Mathf.Rad2Deg + "  " + groundNormal);
                    }
                    else
                    {
                        velocity = Vector3.Reflect(velocity, rHit.normal); // reflect the velocity
                        Time -= d0 / velocity.magnitude;
                        //print(UnityEngine.Time.frameCount + " bounce " + groundNormal);
                    }
                }
                else
                {
                    position += velocity.normalized * Distance;
                    fDistanceTraveled += Distance;
                    break;
                }
            }
            angularSpeed = fDistanceTraveled / Radius * Mathf.Rad2Deg;
        }
        else
        {
            groundNormal = -Gravity.normalized; // reset the ground normal to default
            Time = UnityEngine.Time.fixedDeltaTime;
            velocity += Gravity * Time; // sprinkle some gravity onto the velocity
            Distance = velocity.magnitude * Time;
            while (Time > 0.0f) // collision loop
            {
                if (WillCollide()) // if the ball is going to collide with something
                {
                    Vector3 prevPos = position;
                    position = rHit.point - offset; // move the ball to the offset collision point
                    if (prevPos == position) // avoid getting stuck in the loop
                        break;
                    trail.AddPosition(position); // add the new position to the trail renderer
                    float d0 = (position - prevPos).magnitude; // distance traveled
                    Distance -= d0;
                    float t0 = d0 / velocity.magnitude; // time taken to go from starting position to intersection position
                    Time -= t0;
                    velocity -= Gravity * Time; // update the speed
                    if (Vector3.Angle(rHit.normal, Gravity) > 90.0f) // if the ball will land on ground
                    {
                        groundNormal = rHit.normal; // set the ground normal to the collision normal
                        float gravityAngle = Vector3.Angle(-groundNormal, Gravity) * Mathf.Deg2Rad; // get the angle between the ground and gravity
                        Vector3 gravityDir = Vector3.Cross(groundNormal, Vector3.Cross(Gravity, groundNormal)).normalized; // the direction gravity pushes the ball on the surface
                        float velocityAngle = Vector3.Angle(groundNormal, -velocity) * Mathf.Deg2Rad;
                        Vector3 newVelocityDirection = Vector3.Cross(Vector3.Cross(-velocity, groundNormal), groundNormal).normalized; // the projection of the velocity onto a new surface
                        velocity = newVelocityDirection * (velocity.magnitude * Mathf.Sin(velocityAngle))
                            + gravityDir * (Mathf.Sin(gravityAngle) * Gravity.magnitude * Time); // the new velocity on the new surface of any slope
                        Distance = velocity.magnitude * Time; // calculate the new remaining distance before end of frame time
                        //print(UnityEngine.Time.frameCount + " landed " + groundNormal);
                    }
                    else
                    {
                        velocity = Vector3.Reflect(velocity, rHit.normal); // reflect the velocity
                        velocity += Gravity * Time; // re-add the gravity
                        //print("bounce in air");
                    }
                }
                else
                {
                    position += velocity.normalized * Distance;
                    break;
                }
            }
        }
        transform.RotateAround(transform.position, Vector3.Cross(velocity, Gravity.normalized), angularSpeed);
    }
}
