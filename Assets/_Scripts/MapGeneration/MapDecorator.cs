using UnityEngine;

public static class MapDecorator
{
    public static TileType[,] AddBoundaries(TileType[,] oldMap)
    {
        int width = oldMap.GetLength(0);
        int height = oldMap.GetLength(1);

        TileType[,] newMap = (TileType[,])oldMap.Clone();

        for (int x = 0; x < oldMap.GetLength(0); x++)
        {
            for (int y = 0; y < oldMap.GetLength(1); y++)
            {
                if (oldMap[x, y] == TileType.Empty && HasFloorNeighbor(oldMap, x, y))
                {
                    newMap[x, y] = TileType.Boundary;
                }
            }
        }

        return newMap;
    }

    private static bool HasFloorNeighbor(TileType[,] map, int x, int y)
    {
        int width = map.GetLength(0);
        int height = map.GetLength(1);

        Vector2Int[] directions = {
        new Vector2Int(-1, 0), // влево
        new Vector2Int(1, 0),  // вправо
        new Vector2Int(0, -1), // вниз
        new Vector2Int(0, 1)   // вверх
        };

        foreach (var dir in directions)
        {
            int checkX = x + dir.x;
            int checkY = y + dir.y;

            

            if (checkX >= 0 && checkX < width && checkY >= 0 && checkY < height)
            {
                TileType neighborTile = map[checkX, checkY];

                if (neighborTile != TileType.Empty && neighborTile != TileType.Boundary)
                {
                    return true;
                }
            }
        }

        return false;
    }

}
