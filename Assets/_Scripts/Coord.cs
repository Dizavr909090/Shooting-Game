using System;

public struct Coord : IEquatable<Coord>
{
    public int x;
    public int y;

    public Coord(int X, int Y)
    {
        x = X;
        y = Y;
    }

    public static bool operator ==(Coord c1, Coord c2)
    {
        return c1.x == c2.x && c1.y == c2.y;
    }

    public static bool operator !=(Coord c1, Coord c2)
    {
        return !(c1 == c2);
    }

    public bool Equals(Coord other)
    {
        return this == other;
    }

    public override bool Equals(object obj)
    {
        return obj is Coord other && Equals(other);
    }

    public override int GetHashCode()
    {
        return x.GetHashCode() ^ y.GetHashCode();
    }
}
