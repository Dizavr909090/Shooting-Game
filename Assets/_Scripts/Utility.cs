using System;
public static class Utility
{
   public static T[] ShuffleArray<T>(T[] array, int seed)
    {
        Random prng = new Random(seed);

        for (int i = 0; i < array.Length - 1; i++)
        {
            int randomIndex = prng.Next(i, array.Length);
            T tempItem = array[randomIndex];
            array[randomIndex] = array[i];
            array[i] = tempItem; 
        }

        return array;
    }
    public struct Coord
    {
        public int x;
        public int y;

        public Coord(int X, int Y)
        {
            x = X;
            y = Y;
        }
    }
}
