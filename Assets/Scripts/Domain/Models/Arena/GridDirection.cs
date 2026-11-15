public readonly struct GridDirection
{
    public static readonly GridDirection Up = new GridDirection(0, 1);
    public static readonly GridDirection Down = new GridDirection(0, -1);
    public static readonly GridDirection Left = new GridDirection(-1, 0);
    public static readonly GridDirection Right = new GridDirection(1, 0);

    public int DeltaX { get; }
    public int DeltaY { get; }

    private GridDirection(int deltaX, int deltaY)
    {
        DeltaX = deltaX;
        DeltaY = deltaY;
    }

    public GridPosition ApplyTo(GridPosition position)
    {
        return new GridPosition(position.GridX + DeltaX, position.GridY + DeltaY);
    }
}
