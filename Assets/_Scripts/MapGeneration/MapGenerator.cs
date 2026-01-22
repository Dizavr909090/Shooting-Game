using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UIElements;
using static Utility;

[RequireComponent(typeof(MapSpawner))]
public class MapGenerator : MonoBehaviour
{
    [SerializeField] private MapSettings _mapSettings;

    private MapSpawner _spawner;
    private Coord _mapCenter;

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
        ShuffleCoords();
        string holderName = "Generated Map";
        Transform mapHolder = Spawner.CreateMapHolder(holderName, transform);

        


        List<Vector3> tilePositions = GetTitlePosition();
        Spawner.SpawnTiles(tilePositions, _mapSettings.TilePrefab.gameObject, _mapSettings.OutlinePercent, mapHolder);

        List<Vector3> obstaclePositions = GetObstaclePosition();
        Spawner.SpawnObstacles(obstaclePositions, _mapSettings.ObstaclePrefab.gameObject, mapHolder);
    }

    private bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount)
    {
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(_mapCenter);
        mapFlags[_mapCenter.x, _mapCenter.y] = true;

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
                        if (neighbourX >= 0 && neighbourX < obstacleMap.GetLength(0) &&
                            neighbourY >= 0 && neighbourY < obstacleMap.GetLength(1))
                        {
                            if (!mapFlags[neighbourX, neighbourY] && !obstacleMap[neighbourX, neighbourY])
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

        int targetAccessibleTileCount = (int)(_mapSettings.MapSize.x * _mapSettings.MapSize.y - currentObstacleCount);

        return targetAccessibleTileCount == accessibleTileCount;
    }

    private List<Vector3> GetObstaclePosition()
    {
        int obstacleCount = (int)(_mapSettings.MapSize.x * _mapSettings.MapSize.y * _mapSettings.ObstaclePercent);

        int currentObstacleCount = 0;

        bool[,] obstacleMap = new bool[(int)_mapSettings.MapSize.x, (int)_mapSettings.MapSize.y];

        _mapCenter = new Coord((int)_mapSettings.MapSize.x / 2, (int)_mapSettings.MapSize.y / 2);

        List<Vector3> positions = new List<Vector3>();
        for (int i = 0; i < obstacleCount; i++)
        {
            Coord randomCoord = GetRandomCoord();

            obstacleMap[randomCoord.x, randomCoord.y] = true;
            currentObstacleCount++;

            if (randomCoord != _mapCenter && MapIsFullyAccessible(obstacleMap, currentObstacleCount))
            {
                positions.Add(CoordToPosition(randomCoord.x, randomCoord.y));
            }
            else
            {
                obstacleMap[randomCoord.x, randomCoord.y] = false;
                currentObstacleCount--;
            }
        }

        return positions;
    }

    private List<Vector3> GetTitlePosition()
    {
        List<Vector3> positions = new List<Vector3>();
        for (int x = 0; x < _mapSettings.MapSize.x; x++)
        {
            for (int y = 0; y < _mapSettings.MapSize.y; y++)
            {
                positions.Add(CoordToPosition(x, y));
            }
        }

        return positions;
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

    private Vector3 CoordToPosition(int x, int y)
    {
        return new Vector3(-_mapSettings.MapSize.x / 2 + 0.5f + x, 0, -_mapSettings.MapSize.y / 2 + 0.5f + y);
    }
}
