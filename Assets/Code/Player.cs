using UnityEngine;

public abstract class Player : MonoBehaviour
{
    public GameManagerScript.PlayerState State 
    {
        set
        {
            if (_state != value)
                PreviousState = _state;
            _state = value;
        }
        get => _state;
    }
    private GameManagerScript.PlayerState _state;
    public GameManagerScript.PlayerState PreviousState { get; private set; }
    public GameObject Character { get; set; }
    public GameObject Club
    {
        get => _club;
        set
        {
            _club = value;
            ClubScript = _club.GetComponent<ClubScript>();
            ClubScript.Owner = this;
        }
    }
    private GameObject _club;
    public GameObject Ball
    {
        get => _ball;
        set
        {
            _ball = value;
            BallScript = _ball.GetComponent<BallScript>();
            BallScript.Owner = this;
        }
    }
    private GameObject _ball;
    public ClubScript ClubScript { get; private set; }
    public BallScript BallScript { get; private set; }
    public int Score { get; set; }
    public float Pitch { get; set; }
    public float Yaw { get; set; }
    public bool Done { get; set; }
    public float Power { get; set; }
    public Vector3 previousTeePosition { get; set; }

    public abstract void SetState(GameManagerScript.PlayerState state);
}