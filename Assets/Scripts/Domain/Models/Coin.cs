public class Coin
{
    public int Value { get; private set; }
    public GridPosition Position { get; private set; }

    public Coin(GridPosition position, int value = 1)
    {
        Position = position;
        Value = value;
    }
}