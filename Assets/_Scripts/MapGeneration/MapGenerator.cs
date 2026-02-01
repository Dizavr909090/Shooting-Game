using System;
using Unity.AI.Navigation;
using UnityEngine;
using static MapSettings;

[RequireComponent(typeof(MapSpawner))]
public class MapGenerator : MonoBehaviour
{
    public event Action<LevelMapData> MapGenerated;

    [SerializeField] private MapSettings _mapSettings;
    [SerializeField] private string _holderName = "Generated Map";
    [SerializeField] private NavMeshSurface _navMeshSurface;

    [SerializeField] private int _mapIndex;

    private LevelMapData _currentMapData;
    private MapSpawner _spawner;
    private MapConfig _currentMap;

    public MapGrid Grid { get; private set; }
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

    public void GenerateMap()
    {
        _currentMap = _mapSettings.Maps[_mapIndex];

        MapBuilder mapBuilder = new MapBuilder(_currentMap);
        TileType[,] mapTiles = mapBuilder.Build();

        Grid = new MapGrid(_currentMap.MapSize, _currentMap.TileSize);

        _currentMapData = new LevelMapData(mapTiles, _currentMap.MapSize, _currentMap.TileSize, _currentMap.Seed, Grid);

        Transform mapHolder = Spawner.CreateMapHolder(_holderName, transform);
        Spawner.SpawnMap(_currentMapData, _mapSettings, _currentMap, mapHolder);

        NavMeshSurface.BuildNavMesh();

        MapGenerated?.Invoke(_currentMapData);
    }
}
