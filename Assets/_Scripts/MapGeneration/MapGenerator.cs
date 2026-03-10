using System;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEngine;
using static MapSettings;

public class MapGenerator : MonoBehaviour
{
    public event Action<LevelMapData> MapGenerated;

    [System.Serializable]
    public struct MeshReferences
    {
        public MapMesh Floor;
        public MapMesh Obstacle;
        public MapMesh Boundary;
        public MapMesh Lava;
    }

    [SerializeField] private MeshReferences _meshes;

    [Header("Map")]
    [SerializeField] private MapSettings _mapSettings;
    [SerializeField] private string _holderName = "Generated Map";
    [SerializeField] private NavMeshSurface _navMeshSurface;
    [SerializeField] private int _mapIndex;

    private List<Coord> _debugExits;

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

        _debugExits = mapBuilder.ЕxitsList;

        float pivotX = (_currentMap.MapSize.x - 1) / 2f + mapBuilder.LastHOffset;
        float pivotY = (_currentMap.MapSize.y - 1) / 2f + mapBuilder.LastVOffset;

        Vector2 finalSize = new Vector2(mapTiles.GetLength(0), mapTiles.GetLength(1));
        Grid = new MapGrid(finalSize, _currentMap.TileSize, new Vector2(pivotX, pivotY));

        _currentMapData = new LevelMapData(mapTiles, _currentMap.MapSize, _currentMap.TileSize, _currentMap.Seed, Grid);

        Transform mapHolder = CreateMapHolder(_holderName, transform);

        GenerateMesh();

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

    private void GenerateMesh()
    {
        _meshes.Floor.GenerateMesh(_currentMapData, _mapSettings, _currentMap, TileType.Floor);
        _meshes.Obstacle.GenerateMesh(_currentMapData, _mapSettings, _currentMap, TileType.Obstacle);
        _meshes.Boundary.GenerateMesh(_currentMapData, _mapSettings, _currentMap, TileType.Boundary);
        _meshes.Lava.GenerateMesh(_currentMapData, _mapSettings, _currentMap, TileType.Empty);
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
