using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapSettings", menuName = "Scriptable Objects/MapSettings")]
public class MapSettings : ScriptableObject
{
    [SerializeField] private MapConfig[] _maps;

    [field: SerializeField] public GameObject LavaPrefab { get; private set; }

    public MapConfig[] Maps => _maps;

    [System.Serializable]
    public struct MapConfig
    {
        [SerializeField] private Vector2 _mapSize;
        [SerializeField] private int _seed;
        [SerializeField] private float _tileSize;
        [SerializeField] private float _minObstacleHeight;
        [SerializeField] private float _maxObstacleHeight;
        [SerializeField] private Color _foregroundColour;
        [SerializeField] private Color _backgroundColour;

        [SerializeField, Range(0, 1)]
        private float _outlinePercent;

        [SerializeField, Range(0, 1)]
        private float _obstaclePercent;

        

        [field: SerializeField] public List<RoomPreset> Rooms;
        [field: SerializeField] public int SelectedExitIndex;

        public Vector2 MapSize => _mapSize;
        public int Seed => _seed;
        public float TileSize => _tileSize;
        public float OutlinePercent => _outlinePercent;
        public float ObstaclePercent => _obstaclePercent;
        public int TotalTiles => (int)(MapSize.x * MapSize.y);
        public int TargetObstacleCount => (int)(TotalTiles * ObstaclePercent);
        public Coord MapCenter => new Coord((int)MapSize.x / 2, (int)MapSize.y / 2);
        public float MinObstacleHeight => _minObstacleHeight;
        public float MaxObstacleHeight => _maxObstacleHeight;
        public Color ForegroundColour => _foregroundColour;
        public Color BackgroundColour => _backgroundColour;
    }
}

public enum TileType { Floor, Obstacle, Boundary, Lava, Empty }

public struct LevelMapData
{
    public TileType[,] tileMap;
    public Vector2 mapSize;
    public MapGrid grid;
    private Queue<Coord> _shuffledOpenCoords;

    public Vector3 WorldCenter
    {
        get
        {
            float fx = (tileMap.GetLength(0) - 1) / 2f;
            float fy = (tileMap.GetLength(1) - 1) / 2f;
            return grid.CoordToWorld(fx, fy);
        }
    }

    public LevelMapData(TileType[,] TileMap, Vector2 MapSize, float tileSize, int seed, MapGrid Grid)
    {
        tileMap = TileMap;
        mapSize = MapSize;
        grid = Grid;

        List<Coord> openCoords = new List<Coord>();

        for (int x = 0; x < tileMap.GetLength(0); x++)
        {
            for (int y = 0; y < tileMap.GetLength(1); y++)
            {
                if (tileMap[x, y] == TileType.Floor) openCoords.Add(new Coord(x, y));
            }
        }

        _shuffledOpenCoords = new Queue<Coord>(Utility.ShuffleArray(openCoords.ToArray()));
    }

    public Coord GetRandomOpenTile()
    {
        Coord coord = _shuffledOpenCoords.Dequeue();
        _shuffledOpenCoords.Enqueue(coord);
        return coord;
    }
}
