using System;
using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public event Action EnemyDeath;

    [SerializeField] private Enemy _enemyPrefab;
    [SerializeField] private float _spawnDelay = 1f;
    [SerializeField] private float _tileFlashSpeed = 4f;
    [SerializeField] private MapSpawner _mapSpawner;

    private ICampingProvider _campingProvider;
    private LevelMapData _mapData;
    private bool _hasMapData;
    private ITargetable _target;

    private bool _isDisabled;

    private void Start()
    {
        if (_mapSpawner == null) _mapSpawner = FindFirstObjectByType<MapSpawner>();
    }

    public void Disable() => _isDisabled = true;

    public void Initialize(ICampingProvider campingProvider, ITargetable target)
    {
        _campingProvider = campingProvider;
        _target = target;
    }


    public void OnMapGenerated(LevelMapData data)
    {
        _mapData = data;
        _hasMapData = true;
    }

    private IEnumerator SpawnEnemySequence(Coord spawnCoord)
    {
        Vector3 spawnPosition = _mapData.grid.CoordToWorld(spawnCoord);
 
        Transform tileTransform = _mapSpawner.GetTileAt(spawnCoord.x, spawnCoord.y);

        Renderer tileRenderer = tileTransform.GetComponent<Renderer>();
        Material tileMat = tileRenderer.material;

        Color initialColour = tileMat.color;
        Color flashColour = Color.red;
        float spawnTimer = 0;

        while (spawnTimer < _spawnDelay)
        {
            tileMat.color = Color.Lerp(initialColour, flashColour, Mathf.PingPong(spawnTimer * _tileFlashSpeed, 1));
            spawnTimer += Time.deltaTime;
            yield return null;
        }

        tileMat.color = initialColour;

        Enemy spawnedEnemy = Instantiate(_enemyPrefab, spawnPosition + Vector3.up, Quaternion.identity);

        if (_target != null)
            spawnedEnemy.SetTarget(_target);

        spawnedEnemy.OnDeath += OnEnemyDeath;
    }

    public void SpawnEnemy()
    {
        if (_isDisabled || !_hasMapData) return;

        if (_campingProvider == null) return;

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
