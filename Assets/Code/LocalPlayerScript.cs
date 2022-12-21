using System.Collections.Generic;
using UnityEngine;

public class LocalPlayerScript : Player
{
    private Transform Cam;
    private static LineRendererScript LRS;
    private const int cameraLayerMask = ~((1 << 10) + (1 << 2));
    private Vector3 cameraOffset = new Vector3(0.0f, 2.0f, -1.5f);
    private float followDistance = 2.5f;
    private float predictionDistance = 3.0f;

    private float startTime;
    private float transitionTime = 0.6f;
    private float powerTime = 1.2f;
    private float pivotSpeed = -3.0f;
    private Vector3 positionBefore;
    private Vector3 rotationBefore;
    private Vector3 positionAfter;
    private Vector3 resultPivot;

    private void Start()
    {
        Cam = Camera.main.transform;
        LRS = GetComponent<LineRendererScript>();
    }

    private void Update()
    {
        if (Time.timeScale == 0.0f)
            return;
        switch (GameManagerScript.CurrentGameState)
        {
            case GameManagerScript.GameState.Menu:
                // pivot
                Cam.RotateAround(GameManagerScript.MenuPivot, Vector3.up, pivotSpeed * Time.deltaTime);
                GameManagerScript.MenuStartPosition = Cam.position;
                break;
            case GameManagerScript.GameState.LocalSolo:
                switch (State)
                {
                    case GameManagerScript.PlayerState.Direction:
                        DirectionControls();
                        break;
                    case GameManagerScript.PlayerState.Power:
                        PowerControls();
                        break;
                    case GameManagerScript.PlayerState.Watch:
                        WatchControls();
                        break;
                    case GameManagerScript.PlayerState.Result:
                        ResultControls();
                        break;
                    case GameManagerScript.PlayerState.Spectate:
                        FreeLook(GameManagerScript.CurrentBall.transform.position, 0.0f, 90.0f);
                        break;
                }
                break;
            default:
                throw new System.NotImplementedException();
        }
    }

    private void DirectionControls()
    {
        // input
        if (Input.GetButtonDown("Fire1"))
        {
            GameManagerScript.IncrementPlayerState();
            return;
        }
        Yaw += Input.GetAxisRaw("Horizontal") * GameManagerScript.Sensitivity;
        Pitch -= Input.GetAxisRaw("Vertical") * GameManagerScript.Sensitivity;
        Pitch = Mathf.Max(Mathf.Min(Pitch, 89.9f), -70.0f);
        // camera + club
        Club.transform.RotateAround(Ball.transform.position, Vector3.up, Yaw - Cam.eulerAngles.y);
        Cam.RotateAround(Ball.transform.position + new Vector3(0.0f, cameraOffset.y, 0.0f), Vector3.up, Yaw - Cam.eulerAngles.y);
        Cam.RotateAround(Ball.transform.position + new Vector3(0.0f, cameraOffset.y, 0.0f), Cam.right, Pitch - Cam.eulerAngles.x);
        // prediction line
        CreatePrediction();
    }

    private void PowerControls()
    {
        // input
        if (Input.GetButtonDown("Fire1"))
        {
            GameManagerScript.IncrementPlayerState();
            return;
        }
        else if (Input.GetButtonDown("Fire2"))
        {
            Power = ClubScript.MaxPower;
            GameManagerScript.IncrementPlayerState();
            UIScript.SetPowerLevel(Power / ClubScript.MaxPower);
            return;
        }
        // power
        SetPower();
        UIScript.SetPowerLevel(Power / ClubScript.MaxPower);
    }

    private void WatchControls()
    {
        float frac = (Time.time - startTime) / ClubScript.WindUpTime;
        if (frac < 1.0f)
        {
            // transition
            Cam.position = Vector3.Lerp(positionBefore, positionAfter, frac);
            Cam.eulerAngles = Vector3.Lerp(rotationBefore, new Vector3(Pitch, Yaw, 0.0f), frac);
        }
        else
        {
            if (BallScript.NotMoving || BallScript.InWater || Done)
            {
                print("Result");
                // exit
                GameManagerScript.IncrementPlayerState();
                return;
            }
            FreeLook(Ball.transform.position, 0.0f, 90.0f);
        }
    }

    private void FreeLook(Vector3 pivot, float minPitch, float maxPitch)
    {
        Yaw += Input.GetAxisRaw("Horizontal") * GameManagerScript.Sensitivity;
        Pitch -= Input.GetAxisRaw("Vertical") * GameManagerScript.Sensitivity;
        followDistance -= Input.GetAxis("Mouse ScrollWheel") * 2.0f;
        Pitch = Mathf.Max(Mathf.Min(Pitch, maxPitch), minPitch);
        followDistance = Mathf.Max(Mathf.Min(followDistance, 20.5f), 0.5f);
        Cam.eulerAngles = new Vector3();
        Cam.position = pivot - Vector3.forward * followDistance;
        Cam.RotateAround(pivot, Vector3.up, Yaw);
        Cam.RotateAround(pivot, Cam.right, Pitch);
        if (Physics.Raycast(pivot, (Cam.position - pivot).normalized, out RaycastHit cHit, followDistance, cameraLayerMask))
            Cam.position = cHit.point + Cam.forward * 0.1f;
    }

    private void ResultControls()
    {
        // input
        if (Input.GetButtonDown("Fire1"))
        {
            EndTurn();
            GameManagerScript.IncrementPlayerState();
            return;
        }
        float frac = (Time.time - startTime) / transitionTime;
        if (frac < 1.0f)
        {
            // transition
            Cam.position = Vector3.Lerp(positionBefore, positionAfter, frac);
            Cam.eulerAngles = Vector3.Lerp(rotationBefore, new Vector3(Pitch, Yaw, 0.0f), frac);
        }
        else
        {
            // rotate around pivot
            Cam.RotateAround(resultPivot, Vector3.up, 8.0f * pivotSpeed * Time.deltaTime);
        }
    }
    
    public override void SetState(GameManagerScript.PlayerState state)
    {
        switch (state)
        {
            case GameManagerScript.PlayerState.Direction:
                LRS.Show();
                GameManagerScript.FlashPole();
                // set camera
                Pitch = 33.0f;
                if (Physics.Raycast(Ball.transform.position, Vector3.down, out RaycastHit rhit, 1.0f, BallScript.LayerMask))
                    Yaw = rhit.collider.transform.eulerAngles.y;
                else
                    Yaw = GameManagerScript.CurrentLevel.transform.eulerAngles.y;
                Cam.position = Ball.transform.position + cameraOffset;
                Cam.eulerAngles = new Vector3();
                Cam.RotateAround(Ball.transform.position, Vector3.up, Yaw);
                Cam.RotateAround(Ball.transform.position + new Vector3(0.0f, cameraOffset.y, 0.0f), Cam.right, Pitch);
                break;
            case GameManagerScript.PlayerState.Power:
                LRS.Show();
                startTime = Time.time;
                GameManagerScript.StopFlashPole();
                break;
            case GameManagerScript.PlayerState.Watch:
                LRS.Hide();
                if (BallScript.NotMoving)
                {
                    previousTeePosition = GameManagerScript.TeePosition.position;
                    ClubScript.SwingClub(Power, Club.transform.eulerAngles.y);
                    AnimationSetup(30.0f, followDistance, Ball.transform.position);
                }
                break;
            case GameManagerScript.PlayerState.Result:
                LRS.Show();
                if (BallScript.InWater)
                    resultPivot = BallScript.Splash;
                else if (Done)
                    resultPivot = GameManagerScript.CurrentLevel.Finish.transform.position;
                else
                    resultPivot = Ball.transform.position;
                AnimationSetup(45.0f, 6.0f, resultPivot + Vector3.up * 1.0f);
                UIScript.IncrementScore();
                break;
            case GameManagerScript.PlayerState.Spectate:
                LRS.Hide();
                break;
            case GameManagerScript.PlayerState.Menu:
                GameManagerScript.StopFlashPole();
                LRS.Hide();
                break;
        }
        State = state;
    }

    private void AnimationSetup(float pitch, float distance, Vector3 focusPoint)
    {
        startTime = Time.time;
        positionBefore = Cam.position;
        rotationBefore = Cam.rotation.eulerAngles;
        Yaw = rotationBefore.y;
        Pitch = pitch;
        Cam.eulerAngles = new Vector3(Pitch, Yaw, 0.0f);
        Vector3 forward = Cam.forward;
        Cam.eulerAngles = rotationBefore;
        positionAfter = focusPoint - forward * distance;
    }

    private void SetPower()
    {
        float time = Mathf.Abs(((Time.time - startTime) / powerTime + powerTime) % (2.0f * powerTime) / powerTime - 1);
        Power = Mathf.Pow(time, 2.0f) * ClubScript.MaxPower;
    }

    private void CreatePrediction()
    {
        LRS.Clear();
        Vector3 vPoint = Ball.transform.position;
        LRS.Add(Ball.transform.position);
        Vector3 vDir = Club.transform.forward;
        if (Physics.Raycast(vPoint, vDir, out RaycastHit rHit, predictionDistance, BallScript.LayerMask))
            LRS.Add(rHit.point);
        else
            LRS.Add(vPoint + vDir * predictionDistance);
        LRS.Create();
    }

    private void EndTurn()
    {
        if (previousTeePosition == GameManagerScript.CurrentLevel.transform.position)
            GameManagerScript.BackToStart();
        BallScript.MoveBall(previousTeePosition + Vector3.up * BallScript.Radius);
        Vector3 front, right;
        if (Physics.Raycast(Ball.transform.position, Vector3.down, out RaycastHit rhit, 1.0f, BallScript.LayerMask))
        {
            front = rhit.collider.transform.forward;
            right = rhit.collider.transform.right;
            Yaw = rhit.collider.transform.eulerAngles.y;
        }
        else
        {
            front = GameManagerScript.CurrentLevel.transform.forward;
            right = GameManagerScript.CurrentLevel.transform.right;
            Yaw = GameManagerScript.CurrentLevel.transform.eulerAngles.y;
        }
        Club.transform.position = Ball.transform.position + Vector3.Project(new Vector3(0.032f, 1.747f, -0.104f), front);
    }

    private static float Map(float value, float oldMin, float oldMax, float newMin, float newMax)
    {
        return (value - oldMin) / (oldMax - oldMin) * (newMax - newMin) + newMin;
    }

    private static float CurveMap(float value, float max, float exp)
    {
        return Mathf.Pow(value / max, exp) * max;
    }
}