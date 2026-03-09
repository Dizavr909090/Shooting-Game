using System.Collections.Generic;
using UnityEngine;
using static MapSettings;

public class MapBuilder
{
    private MapConfig _config;
    private TileType[,] _map;
    private bool[,] _mapFlags;

    private List<Coord> _allCoords;
    private Queue<Coord> _shuffledCoords;
    private List<Coord> _allFreeEdgeTiles = new List<Coord>();

    public List<Coord> ЕxitsList { get; private set; } = new List<Coord>();

    public MapBuilder(MapConfig config)
    {
        _config = config;
        _map = new TileType[(int)config.MapSize.x, (int)config.MapSize.y];
        _mapFlags = new bool[(int)config.MapSize.x, (int)config.MapSize.y];

        PrepareShuffledCoords();
    }

    public TileType[,] Build()
    {
        FillFloor();
        PlaceObstacles();

        ЕxitsList = FindFreeEdgeTiles();

        if (_config.Rooms.Count > 0)
        {

            int index = Mathf.Clamp(_config.SelectedExitIndex, 0, ЕxitsList.Count - 1);
            ExpandMap(ЕxitsList[index]);
        }
            

        return _map;
    }

    private void PlaceObstacles()
    {
        int currentObstacleCount = 0;
        Coord mapCenter = _config.MapCenter;

        for (int i = 0; i < _config.TargetObstacleCount; i++)
        {
            Coord randomCoord = GetRandomCoord();

            // for 3x3 center later
            //if (Vector2.Distance(new Vector2(randomCoord.x, randomCoord.y),
            //        new Vector2(mapCenter.x, mapCenter.y)) < 1.5f)
            //{
            //    continue;
            //}

            if (randomCoord.x == mapCenter.x && randomCoord.y == mapCenter.y)
            {
                continue;
            }

            _map[randomCoord.x, randomCoord.y] = TileType.Obstacle;
            currentObstacleCount++;

            if (!MapIsFullyAccessible(currentObstacleCount))
            {
                _map[randomCoord.x, randomCoord.y] = TileType.Floor;
                currentObstacleCount--;
            }
        }
    }

    private bool MapIsFullyAccessible(int currentObstacleCount)
    {
        int targetAccessibleTileCount = _config.TotalTiles - currentObstacleCount;

        int actualAccessibleCount = MapValidator.GetAccessibleTileCount(_map, _config.MapCenter, _mapFlags);

        return targetAccessibleTileCount == actualAccessibleCount;
    }

    private void PrepareShuffledCoords()
    {
        _allCoords = new List<Coord>();

        for (int x = 0; x < _config.MapSize.x; x++)
        {
            for (int y = 0; y < _config.MapSize.y; y++)
            {
                _allCoords.Add(new Coord(x, y));
            }
        }

        _allCoords = new List<Coord>(Utility.ShuffleArray(_allCoords.ToArray(), _config.Seed));

        _shuffledCoords = new Queue<Coord>(_allCoords);
    }

    private void FillFloor()
    {
        for (int x = 0; x < _map.GetLength(0); x++)
        {
            for (int y = 0; y < _map.GetLength(1); y++)
            {
                _map[x, y] = TileType.Floor;
            }
        }
    }

    private List<Coord> FindFreeEdgeTiles()
    {
        _allFreeEdgeTiles.Clear();

        int width = _map.GetLength(0);
        int height = _map.GetLength(1);

        for (int x = 0; x < width; x++) CheckAndAdd(x, 0);

        for (int y = 1; y < height; y++) CheckAndAdd(width - 1, y);

        for (int x = width - 2; x >= 0; x--) CheckAndAdd(x, height - 1);

        for (int y = height - 2; y >= 1; y--) CheckAndAdd(0, y);

        return _allFreeEdgeTiles;
    }

    private void CheckAndAdd(int x, int y)
    {
        var newCoord = new Coord(x, y);

        if (_map[x, y] == TileType.Floor && !_allFreeEdgeTiles.Contains(newCoord))
        {
            _allFreeEdgeTiles.Add(newCoord);
        }
    }

    private void ExpandMap(Coord entrance)
    {
        var oldWidth = _map.GetLength(0);
        var oldHeight = _map.GetLength(1);

        var room = _config.Rooms[0];
        int roomWidth = room.Size.x;
        int roomHeight = room.Size.y;

        int xStart = 0;
        int yStart = 0;

        if (entrance.x == 0)// ЛЕВАЯ СТЕНА
        {
            xStart = -roomWidth;
            yStart = entrance.y - (roomHeight / 2);
        }
        else if (entrance.x == oldWidth - 1)// ПРАВАЯ СТЕНА
        {
            xStart = oldWidth;
            yStart = entrance.y - (roomHeight / 2);
        }
        else if (entrance.y == 0)// НИЖНЯЯ СТЕНА
        {
            xStart = entrance.x - (roomWidth / 2);
            yStart = -roomHeight;
        }
        else if (entrance.y == oldHeight - 1)// ВЕРХНЯЯ СТЕНА
        {
            xStart = entrance.x - (roomWidth / 2);
            yStart = oldHeight;
        }

        int minX = Mathf.Min(0, xStart);
        int minY = Mathf.Min(0, yStart);
        int maxX = Mathf.Max(oldWidth, xStart + roomWidth);
        int maxY = Mathf.Max(oldHeight, yStart + roomHeight);

        int newWidth = maxX - minX;
        int newHeight = maxY - minY;

        int hOffset = -minX;
        int vOffset = -minY;

        TileType[,] newMap = new TileType[newWidth, newHeight];

        for (int x = 0; x < newWidth; x++)
            for (int y = 0; y < newHeight; y++)
                newMap[x, y] = TileType.Empty;

        for (int x = 0; x < oldWidth; x++)
        {
            for (int y = 0; y < oldHeight; y++)
            {
                newMap[x + hOffset, y + vOffset] = _map[x, y];
            }
        }

        for (int x = 0; x < roomWidth; x++)
        {
            for (int y = 0; y < roomHeight; y++)
            {
                int drawX = x + xStart + hOffset;
                int drawY = y + yStart + vOffset;

                newMap[drawX, drawY] = TileType.Floor;
            }
        }

        _map = newMap;
    }

    private Coord GetRandomCoord()
    {
        Coord coord = _shuffledCoords.Dequeue();
        _shuffledCoords.Enqueue(coord);
        return coord;
    }
}
