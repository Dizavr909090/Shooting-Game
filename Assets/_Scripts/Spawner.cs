using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpawnVisualizer))]
public class Spawner : MonoBehaviour
{
    public event Action EnemyDeath;

    [SerializeField] private Enemy _enemyPrefab;
    [SerializeField] private float _spawnDelay = 1f;

    private MapSpawner _mapSpawner;
    private SpawnVisualizer _spawnViisualizer;
    private ICampingProvider _campingProvider;
    private LevelMapData _mapData;
    private bool _hasMapData;
    private ITargetable _target;

    private bool _isDisabled;

    private void Start()
    {
        if (_mapSpawner == null)
            _mapSpawner = GetComponent<MapSpawner>();

        if (_spawnViisualizer == null)
            _spawnViisualizer = GetComponent<SpawnVisualizer>();
    }

    public void Disable() => _isDisabled = true;

    public void Initialize(ICampingProvider campingProvider, ITargetable target, MapSpawner mapSpawner)
    {
        _campingProvider = campingProvider;
        _target = target;
        _mapSpawner = mapSpawner;
    }


    public void OnMapGenerated(LevelMapData data)
    {
        _mapData = data;
        _hasMapData = true;
    }

    private IEnumerator SpawnEnemySequence(Coord spawnCoord)
    {
        var renderer = _mapSpawner.GetTileAt(spawnCoord.x, spawnCoord.y).GetComponent<Renderer>();
        yield return _spawnViisualizer.StartBlink(renderer, _spawnDelay);

        Vector3 spawnPosition = _mapData.grid.CoordToWorld(spawnCoord);

        Enemy spawnedEnemy = Instantiate(_enemyPrefab, spawnPosition + Vector3.up, Quaternion.identity);

        if (_target != null)
            spawnedEnemy.SetTarget(_target);

        spawnedEnemy.OnDeath += OnEnemyDeath;
    }

    public void SpawnEnemy()
    {
        if (_isDisabled || !_hasMapData) return;

        Coord spawnCoord = _mapData.GetRandomOpenTile();

        if (_campingProvider != null && _campingProvider.IsCamping)
        {
            spawnCoord = _mapData.grid.WorldToCoord(_target.Transform.position);
        }

        if (_campingProvider == null) Debug.LogWarning("Spawner: CampingProvider is missing! Camping check disabled.");

        StartCoroutine(SpawnEnemySequence(spawnCoord));
    }

    private void OnEnemyDeath(Entity entity)
    {
        entity.OnDeath -= OnEnemyDeath;

        EnemyDeath?.Invoke();
    }
}
