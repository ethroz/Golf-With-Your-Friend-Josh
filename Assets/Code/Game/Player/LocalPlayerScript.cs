using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Extensions;
using States;

public class LocalPlayerScript : Player
{
    public override int Score 
    { 
        get => score;
        set
        {
            score = value;
            Game.UI.SetScore(score);
        }
    }
    private int score;

    private Transform Cam => Camera.main.transform;

    private Vector2 rotation;
    private Vector3 translation;
    private float zoom;
    private bool shoot;
    private LineRendererScript LRS;
    private const int CAMERA_MASK = ~((1 << 10) + (1 << 2));
    private Vector3 cameraOffset = new(0.0f, 2.0f, -1.5f);
    private float minFollowDistance = 0.5f;
    private float maxFollowDistance = 20.5f;
    private float followDistance = 2.5f;
    private const float PREDICTION_DISTANCE = 3.0f;
    private const int NUM_PREDICTIONS = 1;
    private const float BALL_TOLERANCE = 0.01f;
    private Vector3 previousTeePosition;
    private float power;
    private float sensitivity;

    private float startTime;
    private const float resultAnimationDuration = 0.8f;
    private const float fullChargeInterval = 1.2f;
    private const float resultPivotSpeed = -3.0f;
    private readonly SimpleBezier<float> powerBezier = new(new float[]{0.0f, 0.0f, 0.2f, 1.0f});
    private readonly SimpleBezier<float> easeInOut = new(new float[]{0.0f, 0.0f, 1.0f, 1.0f});
    private readonly SimpleBezier<float> easeOut = new(new float[]{0.0f, 0.5f, 1.0f, 1.0f});
    private readonly SimpleBezier<float> easeOutMoreThanIn = new(new float[]{0.0f, 0.1f, 1.0f, 1.0f});
    private SimpleBezier<Vector3> positionBezier;
    private SimpleBezier<Vector3> rotationBezier;
    private Vector3 positionBefore;
    private Vector3 rotationBefore;
    private Vector3 positionAfter;
    private Vector3 resultPivot;

    private void Awake()
    {
        this.GetComponentInChildrenOrThrow(out LRS);
        Game.Settings.SetCallback(Settings.Sensitivity, (float sens) => { sensitivity = sens; });
    }

    private void Start()
    {
        LRS.Show(false);
    }

    private void Update()
    {
        if (!HasControl)
            return;
        switch (State)
        {
            case PlayerState.Direction: DirectionControls(); break;
            case PlayerState.Power:     PowerControls();     break;
            case PlayerState.Watch:     WatchControls();     break;
            case PlayerState.Result:    ResultControls();    break;
            case PlayerState.Spectate:  FreeLook(Game.CurrentBall.transform.position, 0.0f, 90.0f); break;
#if UNITY_EDITOR
            case PlayerState.FreeRoam:  Roam();              break;
#endif
        }
    }

    private void LateUpdate()
    {
        rotation = new();
        zoom = 0.0f;
        shoot = false;
    }

    private void OnLook(InputValue value)
    {
        rotation += value.Get<Vector2>() * sensitivity;
    }

    private void OnMove(InputValue value)
    {
        translation = value.Get<Vector3>();
    }

    private void OnZoom(InputValue value)
    {
        zoom += value.Get<float>();
    }

    private void OnShoot(InputValue value)
    {
        shoot ^= value.Get<float>() > 0.5f;
    }

    public override void SetState(PlayerState state)
    {
        base.SetState(state);
        switch (state)
        {
            case PlayerState.Direction:
                LRS.Show(true);
                SetTee();
                Cam.position = Ball.transform.position + cameraOffset;
                Cam.eulerAngles = new Vector3();
                Cam.RotateAround(Ball.transform.position, Vector3.up, yaw);
                Cam.RotateAround(Ball.transform.position + new Vector3(0.0f, cameraOffset.y, 0.0f), Cam.right, pitch);
                Game.UI.SetState(MenuState.Game);
                break;
            case PlayerState.Power:
                LRS.Show(true);
                Game.StopFlashPole();
                startTime = Time.time;
                Game.UI.SetPowerLevel(0.0f);
                Game.UI.SetState(MenuState.PowerBar);
                break;
            case PlayerState.Watch:
                LRS.Show(false);
                ClubScript.SwingClub(power, Club.transform.eulerAngles.y);
                AnimationSetup(30.0f, followDistance, Ball.transform.position);
                Game.UI.SetState(MenuState.Game);
                ++Score;
                break;
            case PlayerState.Result:
                LRS.Show(false);
                if (BallScript.InWater)
                    resultPivot = BallScript.Splash;
                else if (Done)
                    resultPivot = Game.CurrentLevel.Finish.transform.position;
                else
                    resultPivot = Ball.transform.position;
                AnimationSetup(45.0f, 6.0f, resultPivot + Vector3.up * 1.0f);
                Game.UI.SetState(MenuState.Continue);
                break;
            case PlayerState.Spectate:
                LRS.Show(false);
                if (!BallScript.Respawn && !BallScript.InWater)
                {
                    previousTeePosition = Ball.transform.position;
                }
                Game.UI.SetState(MenuState.Game);
                break;
        }
    }

    public override void NewLevel(Transform level)
    {
        base.NewLevel(level);
        previousTeePosition = Ball.transform.position;
    }

    private void DirectionControls()
    {
        // Check that the ball has not moved.
        if (Vector3.SqrMagnitude(Ball.transform.position - previousTeePosition) > MathE.Square(BALL_TOLERANCE))
        {
            // Wait for the ball to stop moving.
            if (BallScript.GetSpeed() > 0.0f)
                return;
            previousTeePosition = Ball.transform.position;
            SetState(PlayerState.Direction);
            return;
        }
        if (shoot)
        {
            SetState(PlayerState.Power);
            return;
        }
        yaw += rotation.x;
        pitch += rotation.y;
        pitch = Math.Clamp(pitch, -70.0f, 89.9f);
        // camera + club
        Club.transform.RotateAround(previousTeePosition, Vector3.up, yaw - Cam.eulerAngles.y);
        Cam.RotateAround(previousTeePosition + new Vector3(0.0f, cameraOffset.y, 0.0f), Vector3.up, yaw - Cam.eulerAngles.y);
        Cam.RotateAround(previousTeePosition + new Vector3(0.0f, cameraOffset.y, 0.0f), Cam.right, pitch - Cam.eulerAngles.x);
        // prediction line
        LRS.Set(CreatePrediction(Ball.transform.position, Club.transform.forward, PREDICTION_DISTANCE, NUM_PREDICTIONS));
    }

    private void PowerControls()
    {
        // input
        if (shoot)
        {
            SetState(PlayerState.Watch);
            return;
        }
        // power
        SetPower();
        Game.UI.SetPowerLevel(power / ClubScript.MaxPower);
    }

    private void WatchControls()
    {
        float frac = (Time.time - startTime) / ClubScript.WindUpTime;
        if (frac < 1.0f)
        {
            // transition
            DoAnimation(frac);
        }
        else if (ClubScript.Hit)
        {
            if (!BallScript.IsMoving || BallScript.InWater || Done)
            {
                // exit
                SetState(PlayerState.Result);
                return;
            }
            FreeLook(Ball.transform.position, 0.0f, 90.0f);
        }
    }

    private void ResultControls()
    {
        // input
        if (shoot)
        {
            SetState(PlayerState.Spectate);
            EndTurn();
            return;
        }
        float frac = (Time.time - startTime) / resultAnimationDuration;
        if (frac < 1.0f)
        {
            // transition
            DoAnimation(frac);
        }
        else
        {
            // rotate around pivot
            Cam.RotateAround(resultPivot, Vector3.up, 8.0f * resultPivotSpeed * Time.deltaTime);
        }
    }

    private void FreeLook(Vector3 pivot, float minPitch, float maxPitch)
    {
        yaw += rotation.x;
        pitch += rotation.y;
        followDistance += zoom * 2.0f;
        pitch = Math.Clamp(pitch, minPitch, maxPitch);
        followDistance = Math.Clamp(followDistance, minFollowDistance, maxFollowDistance);
        Cam.eulerAngles = new Vector3();
        Cam.position = pivot - Vector3.forward * followDistance;
        Cam.RotateAround(pivot, Vector3.up, yaw);
        Cam.RotateAround(pivot, Cam.right, pitch);
        if (Physics.Raycast(pivot, (Cam.position - pivot).normalized, out RaycastHit cHit, followDistance, CAMERA_MASK))
            Cam.position = cHit.point + Cam.forward * 0.1f;
    }

    private void Roam()
    {
        yaw += rotation.x;
        pitch += rotation.y;
        pitch = Math.Clamp(pitch, -90.0f, 90.0f);
        Cam.eulerAngles = new Vector3(pitch, yaw, 0.0f);
        Cam.Translate(Time.deltaTime * 5.0f * translation);
    }

    private void AnimationSetup(float pitch, float distance, Vector3 focusPoint)
    {
        startTime = Time.time;
        Vector3 positionBefore = Cam.position;
        Vector3 rotationBefore = new (Cam.eulerAngles.x, MathE.ModBetween(Cam.eulerAngles.y, -180.0f, 180.0f), 0.0f);

        base.pitch = pitch;
        yaw = rotationBefore.y;
        Vector3 rotationAfter = new Vector3(pitch, yaw, 0.0f);
        Vector3 forward = Quaternion.Euler(rotationAfter) * Vector3.forward;
        Vector3 positionAfter = focusPoint - forward * distance;

        positionBezier = new(new Vector3[]{positionBefore, positionAfter});
        rotationBezier = new(new Vector3[]{rotationBefore, rotationAfter});
    }

    private void DoAnimation(float t)
    {
        float t1 = easeOutMoreThanIn.At(t);
        Cam.position = positionBezier.At(t1);
        float t2 = easeOut.At(t);
        Cam.eulerAngles = rotationBezier.At(t2);
    }

    private void SetPower()
    {
        float scaledTime = (Time.time - startTime) / fullChargeInterval;
        float wrappedTime = 1.0f - Mathf.Abs((scaledTime % 2.0f) - 1.0f);
        power = powerBezier.At(wrappedTime) * ClubScript.MaxPower;
    }

    public static Vector3[] CreatePrediction(Vector3 origin, Vector3 dir, float distance, int maxBounces)
    {
        List<Vector3> points = new() { origin };
        for (int i = 0; i < maxBounces + 1; ++i)
        {
            if (!Physics.Raycast(points[i], dir, out RaycastHit rHit, distance, BallScript.LayerMask))
            {
                points.Add(points[i] + dir * distance);
                break;
            }
            points.Add(rHit.point);
            dir = Vector3.Reflect(dir, rHit.normal);
            distance -= rHit.distance;
        }
        return points.ToArray();
    }

    private void EndTurn()
    {
        BallScript.MoveBall(previousTeePosition + Vector3.up * BallScript.Radius);
        Vector3 front;
        if (Physics.Raycast(Ball.transform.position, Vector3.down, out RaycastHit rHit, 1.0f, BallScript.LayerMask))
        {
            front = rHit.collider.transform.forward;
            yaw = rHit.collider.transform.eulerAngles.y;
        }
        else
        {
            front = Game.CurrentLevel.transform.forward;
            yaw = Game.CurrentLevel.transform.eulerAngles.y;
        }
        Club.transform.position = Ball.transform.position + Vector3.Project(new Vector3(0.032f, 1.747f, -0.104f), front);

        Game.NextPlayer();
    }
}
