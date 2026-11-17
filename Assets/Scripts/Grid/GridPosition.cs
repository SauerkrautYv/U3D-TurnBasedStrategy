
using System;

public struct GridPosition : IEquatable<GridPosition>
{
    public int x;
    public int z;

    public GridPosition(int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    public override readonly string ToString()
    {
        return $"({x}, {z})";
    }

    public override readonly bool Equals(object obj)
    {
        return obj is GridPosition position 
        && x == position.x && z == position.z;

        // if (obj is GridPosition other)
        // {
        //     return this == other;
        // }
        // return false;
    }

    public readonly bool Equals(GridPosition other)
    {
        return this == other;
    }

    public override readonly int GetHashCode()
    {
        //return (x, z).GetHashCode();
        return HashCode.Combine(x, z);
    }

    public static bool operator ==(GridPosition a, GridPosition b)
    {
        return a.x == b.x && a.z == b.z;
    }

    public static bool operator !=(GridPosition a, GridPosition b)
    {
        return !(a == b);
    }

    public static GridPosition operator +(GridPosition a, GridPosition b)
    {
        return new GridPosition(a.x + b.x, a.z + b.z);
    }

    public static GridPosition operator -(GridPosition a, GridPosition b)
    {
        return new GridPosition(a.x - b.x, a.z - b.z);
    }
}
