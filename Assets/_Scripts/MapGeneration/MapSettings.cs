using UnityEngine;

[CreateAssetMenu(fileName = "MapSettings", menuName = "Scriptable Objects/MapSettings")]
public class MapSettings : ScriptableObject
{
    [SerializeField]
    private Transform _tilePrefab;
    [SerializeField]
    private Transform _obstaclePrefab;
    [SerializeField]
    private Vector2 _mapSize;
    [SerializeField, Range(0, 1)]
    private float _outlinePercent;
    [SerializeField]
    private int _seed = 10;
    [SerializeField]
    private int _obstacleCount = 10;

    public Transform TilePrefab => _tilePrefab;
    public Transform ObstaclePrefab => _obstaclePrefab;
    public Vector2 MapSize => _mapSize;
    public float OutlinePercent => _outlinePercent;
    public int Seed => _seed;
    public int ObstacleCount => _obstacleCount;

}
