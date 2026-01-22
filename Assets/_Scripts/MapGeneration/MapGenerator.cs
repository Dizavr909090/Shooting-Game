using UnityEngine;
using System.Collections.Generic;


[RequireComponent(typeof(MapSpawner))]
public class MapGenerator : MonoBehaviour
{
    [SerializeField] private MapSettings _mapSettings;

    private MapSpawner _spawner;

    private List<Utility.Coord> _allTileCoords;
    private Queue<Utility.Coord> _shuffledTileCoords;

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


        List<Vector3> tilePositions = new List<Vector3>();
        for (int x = 0; x < _mapSettings.MapSize.x; x++)
        {
            for (int y = 0; y < _mapSettings.MapSize.y; y++)
            {
                tilePositions.Add(CoordToPosition(x, y));
            }
        }

        Spawner.SpawnTiles(tilePositions, _mapSettings.TilePrefab.gameObject, _mapSettings.OutlinePercent, mapHolder);

        List<Vector3> obstaclePositions = new List<Vector3>();
        for (int i = 0; i < _mapSettings.ObstacleCount; i++)
        {
            Utility.Coord randomCoord = GetRandomCoord();
            obstaclePositions.Add(CoordToPosition(randomCoord.x, randomCoord.y));
        }

        Spawner.SpawnObstacles(obstaclePositions, _mapSettings.ObstaclePrefab.gameObject, mapHolder);
    }

    private void ShuffleCoords()
    {
        _allTileCoords = new List<Utility.Coord>();
        for (int x = 0; x < _mapSettings.MapSize.x; x++)
        {
            for (int y = 0; y < _mapSettings.MapSize.y; y++)
            {
                _allTileCoords.Add(new Utility.Coord(x, y));
            }
        }

        _shuffledTileCoords = new Queue<Utility.Coord>(Utility.ShuffleArray(_allTileCoords.ToArray(), _mapSettings.Seed));
    }
    private Utility.Coord GetRandomCoord()
    {
        Utility.Coord randomCoord = _shuffledTileCoords.Dequeue();
        _shuffledTileCoords.Enqueue(randomCoord);
        return randomCoord;
    }

    private Vector3 CoordToPosition(int x, int y)
    {
        return new Vector3(-_mapSettings.MapSize.x / 2 + 0.5f + x, 0, -_mapSettings.MapSize.y / 2 + 0.5f + y);
    }

    
}
