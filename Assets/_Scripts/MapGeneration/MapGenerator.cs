using System;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEngine;
using static MapSettings;

public class MapGenerator : MonoBehaviour
{
    public event Action<LevelMapData> MapGenerated;


    [Serializable]
    public struct MeshReferences
    {
        public MapMesh Floor;
        public MapMesh Obstacle;
        public MapMesh Boundary;
        public MapMesh Lava;
    }

    [Header("Map")]
    [SerializeField] private MapSettings _mapSettings;
    [SerializeField] private string _holderName = "Generated Map";
    [SerializeField] private NavMeshSurface _navMeshSurface;
    [SerializeField] private int _mapIndex;
    [SerializeField] private MeshReferences _meshes;

    private LevelMapData _currentMapData;
    private MapConfig _config;
    

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
        _config = _mapSettings.Maps[_mapIndex];

        MapBuilder mapBuilder = new MapBuilder(_config);
        TileType[,] map = mapBuilder.Build();

        Vector2 finalSize = new Vector2(map.GetLength(0), map.GetLength(1));

        Grid = new MapGrid(finalSize, _config.TileSize, _config.WorldCenter);

        _currentMapData = new LevelMapData(map, finalSize, _config.TileSize, _config.Seed, Grid, _config.HubSize);

        Transform mapHolder = CreateMapHolder(_holderName, transform);

        GenerateMesh();

        GenerateNavMesh();

        MapGenerated?.Invoke(_currentMapData);
    }

    private void GenerateNavMesh()
    {
        float mapWidth = _currentMapData.tileMap.GetLength(0);
        float mapHeight = _currentMapData.tileMap.GetLength(1);

        float fullWidth = mapWidth * _config.TileSize;
        float fullDepth = mapHeight * _config.TileSize;

        NavMeshSurface.center = _currentMapData.WorldCenter;

        NavMeshSurface.size = new Vector3(fullWidth, 10f, fullDepth);

        NavMeshSurface.BuildNavMesh();
    }

    private void GenerateMesh()
    {
        _meshes.Floor.GenerateMesh(_currentMapData, _mapSettings, _config, TileType.Floor);
        _meshes.Obstacle.GenerateMesh(_currentMapData, _mapSettings, _config, TileType.Obstacle);
        _meshes.Boundary.GenerateMesh(_currentMapData, _mapSettings, _config, TileType.Boundary);
        _meshes.Lava.GenerateMesh(_currentMapData, _mapSettings, _config, TileType.Empty);
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
