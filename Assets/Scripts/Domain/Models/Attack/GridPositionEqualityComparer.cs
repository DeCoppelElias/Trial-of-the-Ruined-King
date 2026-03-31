using System.Collections.Generic;

public class GridPositionEqualityComparer : IEqualityComparer<GridPosition>
{
    public bool Equals(GridPosition x, GridPosition y)
    {
        if (x == null && y == null) return true;
        if (x == null || y == null) return false;
        return x.GridX == y.GridX && x.GridY == y.GridY;
    }

    public int GetHashCode(GridPosition obj)
    {
        if (obj == null) return 0;
        unchecked
        {
            int hash = 17;
            hash = hash * 31 + obj.GridX.GetHashCode();
            hash = hash * 31 + obj.GridY.GetHashCode();
            return hash;
        }
    }
}
