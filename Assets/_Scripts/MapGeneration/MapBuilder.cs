using System.Collections.Generic;
using static MapSettings;

public class MapBuilder
{
    private MapConfig _config;
    private TileType[,] _map;
    private bool[,] _mapFlags;

    private List<Coord> _allCoords;
    private Queue<Coord> _shuffledCoords;

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

    private Coord GetRandomCoord()
    {
        Coord coord = _shuffledCoords.Dequeue();
        _shuffledCoords.Enqueue(coord);
        return coord;
    }
}
