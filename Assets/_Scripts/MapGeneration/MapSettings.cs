using System.Collections.Generic;
using UnityEngine;
using static Utility;

[CreateAssetMenu(fileName = "MapSettings", menuName = "Scriptable Objects/MapSettings")]
public class MapSettings : ScriptableObject
{
    [SerializeField] private List<TilePrefabConfig> _tileConfigs;

    [SerializeField] private Vector2 _mapSize;
    [SerializeField] private int _seed = 10;
    [SerializeField] private float _tileSize = 1f;


    [SerializeField, Range(0, 1)] 
    private float _outlinePercent;

    [SerializeField, Range(0, 1)] 
    private float _obstaclePercent;

    public Vector2 MapSize => _mapSize;
    public int Seed => _seed;
    public float TileSize => _tileSize;
    public float OutlinePercent => _outlinePercent;
    public float ObstaclePercent => _obstaclePercent;
    public int TotalTiles => (int)(MapSize.x * MapSize.y);
    public int TargetObstacleCount => (int)(TotalTiles * ObstaclePercent);
    public Coord MapCenter => new Coord((int)MapSize.x / 2, (int)MapSize.y / 2);

    public GameObject GetPrefabByType(TileType type)
    {
        foreach (var config in _tileConfigs)
        {
            if (config.type == type) return config.prefab;
        }
        return null;
    }
}

public enum TileType { Floor, Obstacle }

public struct LevelMapData
{
    public TileType[,] tileMap;

    public Vector2 mapSize;
    public Coord mapCenter;

    public LevelMapData(TileType[,] TileMap, Vector2 MapSize, Coord MapCenter)
    {
        tileMap = TileMap;
        mapSize = MapSize;
        mapCenter = MapCenter;
    }
}

[System.Serializable]
public struct TilePrefabConfig
{
    public TileType type;
    public GameObject prefab;
}
