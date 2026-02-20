using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpawnVisualizer), typeof(EnemyPool))]
public class Spawner : MonoBehaviour
{
    public event Action EnemyDeath;

    [SerializeField] private float _spawnDelay = 1f;

    private EnemyPool _enemyPool;
    private MapSpawner _mapSpawner;
    private SpawnVisualizer _spawnVisualizer;
    private ICampingProvider _campingProvider;
    private LevelMapData _mapData;
    private bool _hasMapData;
    private ITargetable _target;

    private bool _isDisabled;

    private void Awake()
    {
        if (_enemyPool  == null)
            _enemyPool = GetComponent<EnemyPool>();

        if (_spawnVisualizer == null)
            _spawnVisualizer = GetComponent<SpawnVisualizer>();
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
        yield return _spawnVisualizer.StartBlink(renderer, _spawnDelay);

        Vector3 spawnPosition = _mapData.grid.CoordToWorld(spawnCoord);

        Enemy spawnedEnemy = _enemyPool.GetEnemy();

        spawnedEnemy.Activate(_target, spawnPosition);

        spawnedEnemy.Health.OnDeath += ReturnEnemyToPool;

        yield return null;
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

    private void ReturnEnemyToPool(IHealth health)
    {
        Enemy enemy = (health as MonoBehaviour)?.GetComponent<Enemy>();

        if (enemy != null)
        {
            enemy.Health.OnDeath -= ReturnEnemyToPool;
            _enemyPool.Return(enemy);
            EnemyDeath?.Invoke();
        }
    }
}
