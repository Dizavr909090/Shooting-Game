using System;
using System.Collections.Generic;

public static class MapValidator
{
    public static int GetAccessibleTileCount(TileType[,] map, Coord startCoord, bool[,] mapFlags)
    {
        Array.Clear(mapFlags, 0, map.Length);
        Queue<Coord> queue = new Queue<Coord>();

        queue.Enqueue(startCoord);
        mapFlags[startCoord.x, startCoord.y] = true;
        int accessibleTileCount = 1;

        while (queue.Count > 0)
        {
            Coord tile = queue.Dequeue();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    int neighbourX = tile.x + x;
                    int neighbourY = tile.y + y;

                    if (x == 0 || y == 0)
                    {
                        if (neighbourX >= 0 && neighbourX < map.GetLength(0) &&
                            neighbourY >= 0 && neighbourY < map.GetLength(1))
                        {
                            if (!mapFlags[neighbourX, neighbourY] && !(map[neighbourX, neighbourY] == TileType.Obstacle))
                            {
                                mapFlags[neighbourX, neighbourY] = true;
                                queue.Enqueue(new Coord(neighbourX, neighbourY));
                                accessibleTileCount++;
                            }
                        }
                    }
                }
            }
        }

        return accessibleTileCount;
    }
}
