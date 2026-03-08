using UnityEngine;

public class MapGrid
{
    private readonly Vector2 _mapSize;
    private readonly float _tileSize;

    public MapGrid(Vector2 mapSize, float tileSize)
    {
        _mapSize = mapSize;
        _tileSize = tileSize;
    }

    public Vector3 CoordToWorld(float xCoord, float yCoord)
    {
        float mapWidth = _mapSize.x * _tileSize;
        float mapHeight = _mapSize.y * _tileSize;

        float startX = -mapWidth / 2f;
        float startZ = -mapHeight / 2f;

        float x = startX + (xCoord * _tileSize) + (_tileSize / 2f);
        float z = startZ + (yCoord * _tileSize) + (_tileSize / 2f);

        return new Vector3(x, 0, z);
    }

    public Vector3 CoordToWorld(Coord coord) => CoordToWorld(coord.x, coord.y);

    public Coord WorldToCoord(Vector3 position)
    {
        float mapWidth = _mapSize.x * _tileSize;
        float mapHeight = _mapSize.y * _tileSize;

        float xLocal = position.x + mapWidth / 2f;
        float zLocal = position.z + mapHeight / 2f;

        int x = Mathf.FloorToInt(xLocal / _tileSize);
        int y = Mathf.FloorToInt(zLocal / _tileSize);

        return new Coord(
            Mathf.Clamp(x, 0, (int)_mapSize.x - 1),
            Mathf.Clamp(y, 0, (int)_mapSize.y - 1)
        );
    }

    public bool IsInside(Coord coord)
    {
        return coord.x >= 0 && coord.x < _mapSize.x && coord.y >= 0 && coord.y < _mapSize.y;
    }
}
