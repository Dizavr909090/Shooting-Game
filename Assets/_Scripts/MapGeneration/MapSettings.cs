using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapSettings", menuName = "Scriptable Objects/MapSettings")]
public class MapSettings : ScriptableObject
{
    [SerializeField] private MapConfig[] _maps;

    public MapConfig[] Maps => _maps;

    [System.Serializable]
    public struct MapConfig
    {
        [field: SerializeField] public Vector2 HubSize;
        [field: SerializeField] public int Seed;
        [field: SerializeField] public float TileSize;
        [field: SerializeField] public float MinObstacleHeight;
        [field: SerializeField] public float MaxObstacleHeight;
        [field: SerializeField] public Color ForegroundColour;
        [field: SerializeField] public Color BackgroundColour;
        [field: SerializeField] public List<RoomPreset> Rooms;
        [field: SerializeField] public int SelectedExitIndex;
        [field: SerializeField] public int MaxGridSize;

        [SerializeField, Range(0, 1)]
        private float _obstaclePercent;

        public float ObstaclePercent => _obstaclePercent;
        public int HubTotalTiles => (int)(HubSize.x * HubSize.y);
        public int TargetObstacleCount => (int)(HubTotalTiles * ObstaclePercent);
        public Coord WorldCenterCoord => new Coord(MaxGridSize / 2, MaxGridSize / 2);
        public Vector2Int WorldCenter => new Vector2Int(MaxGridSize / 2, MaxGridSize / 2);
        public Vector2Int HubLocalCenter => new Vector2Int((int)HubSize.x / 2, (int)HubSize.y / 2);
    }
}

public enum TileType { Floor, Obstacle, Boundary, Lava, Empty }

public struct LevelMapData
{
    public TileType[,] tileMap;
    public Vector2 mapSize;
    public MapGrid grid;
    public Vector2 HubSize;
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

    public LevelMapData(TileType[,] TileMap, Vector2 MapSize, float tileSize, int seed, MapGrid Grid, Vector2 hubSize)
    {
        tileMap = TileMap;
        mapSize = MapSize;
        grid = Grid;
        HubSize = hubSize;

        List<Coord> openCoords = new List<Coord>();

        int width = tileMap.GetLength(0);
        int height = tileMap.GetLength(1);

        int hubW = (int)hubSize.x;
        int hubH = (int)hubSize.y;

        int startX = (width / 2) - (hubW / 2);
        int startY = (height / 2) - (hubH / 2);

        for (int x = startX + 1; x < startX + hubW - 1; x++)
        {
            for (int y = startY + 1; y < startY + hubH - 1; y++)
            {
                if (tileMap[x, y] == TileType.Floor)
                {
                    openCoords.Add(new Coord(x, y));
                }
            }
        }

        _shuffledOpenCoords = new Queue<Coord>(Utility.ShuffleArray(openCoords.ToArray(), seed));
    }

    public Coord GetRandomOpenTile()
    {
        Coord coord = _shuffledOpenCoords.Dequeue();
        _shuffledOpenCoords.Enqueue(coord);
        return coord;
    }
}
