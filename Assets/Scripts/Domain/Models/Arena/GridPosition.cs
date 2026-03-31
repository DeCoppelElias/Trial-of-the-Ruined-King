using System;

[Serializable]
public class GridPosition
{
    public int GridX;
    public int GridY;

    public GridPosition(int gridX, int gridY)
    {
        this.GridX = gridX;
        this.GridY = gridY;
    }

    public GridPosition()
    {
        this.GridX = 0;
        this.GridY = 0;
    }

    public override string ToString()
    {
        return $"({GridX}, {GridY})";
    }

    public override bool Equals(object obj)
    {
        if (obj is GridPosition other)
        {
            return GridX == other.GridX && GridY == other.GridY;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(GridX, GridY);
    }
}