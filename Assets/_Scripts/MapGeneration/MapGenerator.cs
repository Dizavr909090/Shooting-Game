using System;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEngine;
using static MapSettings;

public class MapGenerator : MonoBehaviour
{
    public event Action<LevelMapData> MapGenerated;

    [SerializeField] private MapSettings _mapSettings;
    [SerializeField] private string _holderName = "Generated Map";
    [SerializeField] private NavMeshSurface _navMeshSurface;
    [SerializeField] private MapMesh _floorMesh;
    [SerializeField] private MapMesh _obstacleMesh;
    [SerializeField] private int _mapIndex;

    private LevelMapData _currentMapData;
    private MapConfig _currentMap;

    public MapGrid Grid { get; private set; }
    public MapSettings MapSettings => _mapSettings;
    private NavMeshSurface NavMeshSurface
    {
        get
        {
            if (_navMeshSurface == null) _navMeshSurface = GetComponent<NavMeshSurface>();
            return _navMeshSurface;
        }
    }

    public void OnNewWave(int waveNumber)
    {
        _mapIndex = waveNumber - 1;
        GenerateMap();
    }

    public void GenerateMap()
    {
        _currentMap = _mapSettings.Maps[_mapIndex];

        MapBuilder mapBuilder = new MapBuilder(_currentMap);
        TileType[,] mapTiles = mapBuilder.Build();

        Grid = new MapGrid(_currentMap.MapSize, _currentMap.TileSize);

        _currentMapData = new LevelMapData(mapTiles, _currentMap.MapSize, _currentMap.TileSize, _currentMap.Seed, Grid);

        Transform mapHolder = CreateMapHolder(_holderName, transform);
        _floorMesh.GenerateMesh(_currentMapData, _mapSettings, _currentMap, TileType.Floor);
        _obstacleMesh.GenerateMesh(_currentMapData, _mapSettings, _currentMap, TileType.Obstacle);

        GenerateNavMesh();
     
        MapGenerated?.Invoke(_currentMapData);
    }

    private void GenerateNavMesh()
    {
        float fullWidth = _currentMapData.tileMap.GetLength(0) * _currentMap.TileSize;
        float fullDepth = _currentMapData.tileMap.GetLength(1) * _currentMap.TileSize;

        float centerPointX = (_currentMapData.tileMap.GetLength(0) - 1) / 2f;
        float centerPointY = (_currentMapData.tileMap.GetLength(1) - 1) / 2f;
        NavMeshSurface.center = Grid.CoordToWorld(centerPointX, centerPointY);

        NavMeshSurface.size = new Vector3(fullWidth, 10f, fullDepth);

        NavMeshSurface.BuildNavMesh();
    }

    private Transform CreateMapHolder(string name, Transform parent)
    {
        Transform oldHolder = parent.Find(name);
        if (oldHolder)
        {
#if UNITY_EDITOR
            Undo.DestroyObjectImmediate(oldHolder.gameObject);
#else
            Destroy(oldHolder.gameObject);
#endif
        }
        GameObject gameObject = new GameObject(name);
#if UNITY_EDITOR
        Undo.RegisterCreatedObjectUndo(gameObject, "Generate Map");
#endif
        gameObject.transform.parent = parent;

        return gameObject.transform;
    }
}
