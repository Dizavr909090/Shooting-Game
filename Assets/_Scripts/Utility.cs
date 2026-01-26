using System;
using UnityEngine;

public static class Utility
{
    public static T[] ShuffleArray<T>(T[] array, int seed)
    {
        System.Random prng = new System.Random(seed);

        for (int i = 0; i < array.Length - 1; i++)
        {
            int randomIndex = prng.Next(i, array.Length);
            T tempItem = array[randomIndex];
            array[randomIndex] = array[i];
            array[i] = tempItem; 
        }

        return array;
    }

    public static Vector3 CoordToPosition(Vector2 size, int x, int y, float tileSize)
    {
        return new Vector3((-size.x / 2 + 0.5f + x) * tileSize, 0, (-size.y / 2 + 0.5f + y) * tileSize);
    }

    public static Vector3 CoordToPosition(Vector2 size, int x, int y)
    {
        return new Vector3(-size.x / 2 + 0.5f + x, 0, -size.y / 2 + 0.5f + y);
    }

    public static Coord WorldPositionToCoord(Vector3 worldPos, Vector2 mapSize, float tileSize)
    {
        int x = Mathf.RoundToInt(worldPos.x / tileSize + (mapSize.x - 1) / 2f);
        int y = Mathf.RoundToInt(worldPos.z / tileSize + (mapSize.y - 1) / 2f);

        x = Mathf.Clamp(x, 0, (int)mapSize.x - 1);
        y = Mathf.Clamp(y, 0, (int)mapSize.y - 1);

        return new Coord(x, y);
    }

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
}
