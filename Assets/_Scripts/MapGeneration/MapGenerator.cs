using System.Collections.Generic;
using UnityEngine;
using static Utility;

[RequireComponent(typeof(MapSpawner))]
public class MapGenerator : MonoBehaviour
{
    [SerializeField] private MapSettings _mapSettings;
    [SerializeField] private string _holderName = "Generated Map";

    private MapSpawner _spawner;

    private List<Coord> _allTileCoords;
    private Queue<Coord> _shuffledTileCoords;

    public MapSettings MapSettings => _mapSettings;
    private MapSpawner Spawner
    {
        get
        {
            if (_spawner == null) _spawner = GetComponent<MapSpawner>();
            return _spawner;
        }
    }

    private void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        Transform mapHolder = Spawner.CreateMapHolder(_holderName, transform);

        LevelMapData mapData = GenerateMapData();

        Spawner.SpawnMap(mapData, _mapSettings, mapHolder);
    }

    private LevelMapData GenerateMapData()
    {
        ShuffleCoords();

        int width = (int)_mapSettings.MapSize.x;
        int height = (int)_mapSettings.MapSize.y;
        TileType[,] map = new TileType[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                map[x, y] = TileType.Floor;
            }
        }

        PlaceObstacles(map);

        return new LevelMapData(map, _mapSettings.MapSize, _mapSettings.MapCenter);
    }

    private bool MapIsFullyAccessible(TileType[,] map, int currentObstacleCount)
    {
        int targetAccessibleTileCount = (int)(_mapSettings.TotalTiles - currentObstacleCount);
        int accessibleTileCount = GetAccessibleTileCount(map, _mapSettings.MapCenter);

        return targetAccessibleTileCount == accessibleTileCount;
    }

    private int GetAccessibleTileCount(TileType[,] map, Coord startCoord)
    {
        bool[,] mapFlags = new bool[map.GetLength(0), map.GetLength(1)];
        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(_mapSettings.MapCenter);
        mapFlags[_mapSettings.MapCenter.x, _mapSettings.MapCenter.y] = true;

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

    private void PlaceObstacles(TileType[,] map )
    {
        int currentObstacleCount = 0;

        for (int i = 0; i < _mapSettings.TargetObstacleCount; i++)
        {
            Coord randomCoord = GetRandomCoord();

            map[randomCoord.x, randomCoord.y] = TileType.Obstacle;
            currentObstacleCount++;

            if (randomCoord != _mapSettings.MapCenter && MapIsFullyAccessible(map, currentObstacleCount)) { }
            else
            {
                map[randomCoord.x, randomCoord.y] = TileType.Floor;
                currentObstacleCount--;
            }
        }
    }

    private void ShuffleCoords()
    {
        _allTileCoords = new List<Coord>();

        for (int x = 0; x < _mapSettings.MapSize.x; x++)
        {
            for (int y = 0; y < _mapSettings.MapSize.y; y++)
            {
                _allTileCoords.Add(new Coord(x, y));
            }
        }

        _shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(_allTileCoords.ToArray(), _mapSettings.Seed));
    }

    private Coord GetRandomCoord()
    {
        Coord randomCoord = _shuffledTileCoords.Dequeue();
        _shuffledTileCoords.Enqueue(randomCoord);
        return randomCoord;
    }
}
