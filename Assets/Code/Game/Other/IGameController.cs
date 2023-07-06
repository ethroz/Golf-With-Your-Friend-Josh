public interface IGameController : IGameListener
{
    public abstract bool HasControl { get; }
    public void SetControl(bool control);
}
