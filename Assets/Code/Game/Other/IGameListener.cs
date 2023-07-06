public interface IGameListener
{
    public abstract GameManagerScript Game { get; }
    public void SetManager(GameManagerScript game);
}
