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

            foreach (var dir in MapBuilder.Directions.All)
            {
                int neighbourX = tile.x + dir.x;
                int neighbourY = tile.y + dir.y;

                if (neighbourX >= 0 && neighbourX < map.GetLength(0) &&
                            neighbourY >= 0 && neighbourY < map.GetLength(1))
                {
                    if (!mapFlags[neighbourX, neighbourY] && map[neighbourX, neighbourY] == TileType.Floor)
                    {
                        mapFlags[neighbourX, neighbourY] = true;
                        queue.Enqueue(new Coord(neighbourX, neighbourY));
                        accessibleTileCount++;
                    }
                }
            }
        }

        return accessibleTileCount;
    }
}
