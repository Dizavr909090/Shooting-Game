using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using static MapSettings;
using static Utility;

[RequireComponent(typeof(MapSpawner))]
public class MapGenerator : MonoBehaviour
{
    [SerializeField] private MapSettings _mapSettings;
    [SerializeField] private string _holderName = "Generated Map";
    [SerializeField] private NavMeshSurface _navMeshSurface;

    [SerializeField] private int _mapIndex;

    private Vector2 _lastMapSize;
    private MapSpawner _spawner;
    private MapConfig _currentMap;

    private bool[,] _mapFlags;
    private Queue<Coord> _accessibleQueue;
    private List<Coord> _allTileCoords;
    private Queue<Coord> _shuffledTileCoords;
    private Queue<Coord> _shuffledOpenCoords;

    public MapSettings MapSettings => _mapSettings;
    private MapSpawner Spawner
    {
        get
        {
            if (_spawner == null) _spawner = GetComponent<MapSpawner>();
            return _spawner;
        }
    }
    private NavMeshSurface NavMeshSurface
    {
        get
        {
            if (_navMeshSurface == null) _navMeshSurface = GetComponent<NavMeshSurface>();
            return _navMeshSurface;
        }
    }

    private void Start()
    {
        GenerateMap();
    }

    public Transform GetTileFromPosition(Vector3 position)
    {
        int x = Mathf.RoundToInt(position.x / _currentMap.TileSize + (_currentMap.MapSize.x - 1) / 2f);
        int y = Mathf.RoundToInt(position.z / _currentMap.TileSize + (_currentMap.MapSize.y - 1) / 2f);
        x = Mathf.Clamp(x, 0, (int)_currentMap.MapSize.x - 1);
        y = Mathf.Clamp(y, 0, (int)_currentMap.MapSize.y - 1);

        return Spawner.GetTileAt(x, y);
    }

    public Vector3 PositionFromCoord(Coord coord)
    {
        return Utility.CoordToPosition(_currentMap.MapSize, coord.x, coord.y, _currentMap.TileSize);
    }

    public Coord GetRandomOpenTile()
    {
        Coord randomCoord = _shuffledOpenCoords.Dequeue();
        _shuffledOpenCoords.Enqueue(randomCoord);
        return randomCoord;
    }

    public void GenerateMap()
    {
        Transform mapHolder = Spawner.CreateMapHolder(_holderName, transform);

        LevelMapData mapData = GenerateMapData();

        BoxCollider mapCollider = GetComponent<BoxCollider>();
        mapCollider.size = new Vector3(_currentMap.MapSize.x * _currentMap.TileSize, .5f, _currentMap.MapSize.y * _currentMap.TileSize);
        mapCollider.center = new Vector3(0, -.5f / 2f, 0);

        NavMeshSurface.RemoveData();

        Spawner.SpawnMap(mapData, _mapSettings, _currentMap, mapHolder);

        ShuffleOpenCoords(mapData);

        NavMeshSurface.BuildNavMesh();
    }

    private LevelMapData GenerateMapData()
    {
        _currentMap = _mapSettings.Maps[_mapIndex];

        if (_allTileCoords == null)
        {
            _allTileCoords = new List<Coord>();
        }

        ShuffleCoords();

        int width = (int)_currentMap.MapSize.x;
        int height = (int)_currentMap.MapSize.y;
        TileType[,] map = new TileType[width, height];

        if (_mapFlags == null || _currentMap.MapSize != _lastMapSize)
        {
            _mapFlags = new bool[width, height];
            _accessibleQueue = new Queue<Coord>();
        }


        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                map[x, y] = TileType.Floor;
            }
        }

        PlaceObstacles(map);

        _lastMapSize = _currentMap.MapSize;

        return new LevelMapData(map, _currentMap.MapSize, _currentMap.MapCenter);
    }

    private bool MapIsFullyAccessible(TileType[,] map, int currentObstacleCount)
    {
        int targetAccessibleTileCount = (int)(_currentMap.TotalTiles - currentObstacleCount);
        int accessibleTileCount = GetAccessibleTileCount(map, _currentMap.MapCenter);

        return targetAccessibleTileCount == accessibleTileCount;
    }

    private int GetAccessibleTileCount(TileType[,] map, Coord startCoord)
    {
        System.Array.Clear(_mapFlags, 0, _mapFlags.Length);

        _accessibleQueue.Clear();
        _accessibleQueue.Enqueue(_currentMap.MapCenter);
        _mapFlags[_currentMap.MapCenter.x, _currentMap.MapCenter.y] = true;

        int accessibleTileCount = 1;

        while (_accessibleQueue.Count > 0)
        {
            Coord tile = _accessibleQueue.Dequeue();

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
                            if (!_mapFlags[neighbourX, neighbourY] && !(map[neighbourX, neighbourY] == TileType.Obstacle))
                            {
                                _mapFlags[neighbourX, neighbourY] = true;
                                _accessibleQueue.Enqueue(new Coord(neighbourX, neighbourY));
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

        for (int i = 0; i < _currentMap.TargetObstacleCount; i++)
        {
            Coord randomCoord = GetRandomCoord();

            map[randomCoord.x, randomCoord.y] = TileType.Obstacle;
            currentObstacleCount++;

            if (randomCoord == _currentMap.MapCenter || !MapIsFullyAccessible(map, currentObstacleCount)) 
            {
                map[randomCoord.x, randomCoord.y] = TileType.Floor;
                currentObstacleCount--;
            }
        }
    }

    private void ShuffleCoords()
    {
        int width = (int)_currentMap.MapSize.x;
        int height = (int)_currentMap.MapSize.y;

        if (_currentMap.MapSize != _lastMapSize || _allTileCoords.Count == 0)
        {
            _allTileCoords.Clear();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    _allTileCoords.Add(new Coord(x, y));
                }
            }
        }

        _shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(_allTileCoords.ToArray(), _currentMap.Seed));
    }

    private void ShuffleOpenCoords(LevelMapData mapData)
    {
        _shuffledOpenCoords = new Queue<Coord>(ShuffleArray(mapData.GetOpenCoords().ToArray(), _currentMap.Seed));
    }

    private Coord GetRandomCoord()
    {
        Coord randomCoord = _shuffledTileCoords.Dequeue();
        _shuffledTileCoords.Enqueue(randomCoord);
        return randomCoord;
    }
}
