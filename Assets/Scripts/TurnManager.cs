public class TurnManager
{
    public event System.Action OnTick;

    private int _turnCount;

    public TurnManager()
    {
        _turnCount = 1;
    }

    public void Tick()
    {
        _turnCount += 1;
        OnTick?.Invoke();
    }
}
