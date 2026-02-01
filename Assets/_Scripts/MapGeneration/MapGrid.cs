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

    public Vector3 CoordToWorld(Coord coord)
    {
        return new Vector3(
            (-_mapSize.x / 2f + 0.5f + coord.x) * _tileSize,
            0,
            (-_mapSize.y / 2f + 0.5f + coord.y) * _tileSize
            );
    }

    public Coord WorldToCoord(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt(worldPos.x / _tileSize + (_mapSize.x -1) / 2f);
        int y = Mathf.RoundToInt(worldPos.z / _tileSize + (_mapSize.y - 1) / 2f);

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
