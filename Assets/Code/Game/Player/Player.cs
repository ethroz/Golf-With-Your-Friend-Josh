using UnityEngine;
using States;

public abstract class Player : MonoBehaviour, IGameController
{
    public bool HasControl { get; private set; }
    public GameManagerScript Game { get; private set; }
    public PlayerState State { get; private set; }
    public virtual int Score { get; set; }
    public bool Done { get; set; }

    public GameObject Character { get; set; }
    public GameObject Club
    {
        get => club;
        set
        {
            club = value;
            ClubScript = club.GetComponent<ClubScript>();
            ClubScript.SetPlayer(this);
        }
    }
    public GameObject Ball
    {
        get => ball;
        set
        {
            ball = value;
            BallScript = ball.GetComponent<BallScript>();
            BallScript.SetManager(Game);
        }
    }
    public ClubScript ClubScript { get; private set; }
    public BallScript BallScript { get; private set; }
    private GameObject club;
    private GameObject ball;
    protected float pitch;
    protected float yaw;

    public virtual void SetState(PlayerState state)
    {
        State = state;
    }

    public void SetControl(bool control)
    {
        HasControl = control;
    }

    public void SetManager(GameManagerScript manager)
    {
        Game = manager;
    }

    public virtual void NewLevel(Transform level)
    {
        Score = 0;
        Done = false;
        // ball
        BallScript.MoveBall(level.position + Vector3.up * BallScript.Radius);
        // character
        Character.transform.position = level.position - 1.5f * level.forward - 1.5f * level.right;
    }

    protected virtual void SetTee()
    {
        // direction
        BallScript.StopBall();
        if (!Physics.Raycast(Ball.transform.position, Vector3.down, out RaycastHit rHit, 1.0f, BallScript.LayerMask))
        {
            throw new MissingComponentException("Missing floor to level");
        }
        print(string.Format("On: {0}, {1}", rHit.collider.name, rHit.transform.rotation.eulerAngles));
        yaw = rHit.collider.transform.eulerAngles.y;
        pitch = 33.0f;
        // club
        club.transform.position = ball.transform.position + new Vector3(0.032f, 1.747f, -0.104f);
        club.transform.eulerAngles = new Vector3();
        club.transform.RotateAround(ball.transform.position, Vector3.up, yaw);
        // others
        Character.transform.eulerAngles = new Vector3(0.0f, 45.0f + yaw, 0.0f);
    }
}
