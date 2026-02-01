using System;
using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private Wave[] _waves;
    [SerializeField] private Enemy _enemyPrefab;
    [SerializeField] private float _spawnDelay = 1f;
    [SerializeField] private float _tileFlashSpeed = 4f;
    [SerializeField] private float _campThresholdDistance = 1.5f;
    [SerializeField] private float _timeBetweenCampingChecks = 2;

    private MapSpawner _mapVisualizer;
    private LevelMapData _mapData;
    private bool _hasMapData;
    private Entity _playerEntity;
    private Transform _playerTransform;
    private ITargetable _target;

    private Wave _currentWave;
    private int _currentWaveNumber;

    private int _enemiesRemainingToSpawn;
    private int _enemiesRemainingAlive;
    private float _nextSpawnTime;
    private bool _isDisabled;
    private float _nextCampCheckTime;
    private Vector3 _campPositionOld;
    private bool _isCamping;

    private void Start()
    {
        InitializePlayer();

        NextWave();
    }

    private void Update()
    {
        if (!_isDisabled)
        {
            CampingCheck();
            SpawnEnemy();
        }
    }

    public void OnMapGenerated(LevelMapData data)
    {
        _mapData = data;
        _hasMapData = true;
    }

    private IEnumerator SpawnEnemySequence(Coord spawnCoord)
    {
        Vector3 spawnPosition = _mapData.grid.CoordToWorld(spawnCoord);

        if (_mapVisualizer == null) _mapVisualizer = FindFirstObjectByType<MapSpawner>();
        Transform tileTransform = _mapVisualizer.GetTileAt(spawnCoord.x, spawnCoord.y);

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

    private void SpawnEnemy()
    {
        if (!_hasMapData) return;

        if (_enemiesRemainingToSpawn > 0 && Time.time > _nextSpawnTime)
        {
            _enemiesRemainingToSpawn--;
            _nextSpawnTime = Time.time + _currentWave._timeBetweenSpawns;

            Coord spawnCoord = _mapData.GetRandomOpenTile();

            if (_isCamping)
            {
                spawnCoord = _mapData.grid.WorldToCoord(_playerTransform.position);
            }

            StartCoroutine(SpawnEnemySequence(spawnCoord));
        }
    }

    private void NextWave()
    {
        _currentWaveNumber++;
        
        if (_currentWaveNumber - 1 < _waves.Length)
        {
            print("Wave: " + _currentWaveNumber);
            _currentWave = _waves[_currentWaveNumber - 1];

            _enemiesRemainingToSpawn = _currentWave._enemyCount;
            _enemiesRemainingAlive = _enemiesRemainingToSpawn;
        }
    }

    private void OnEnemyDeath(Entity entity)
    {
        entity.OnDeath -= OnEnemyDeath;

        _enemiesRemainingAlive --;
        if (_enemiesRemainingAlive == 0)
        {
            NextWave();
        }
    }

    private void CampingCheck()
    {
        if (Time.time > _nextCampCheckTime)
        {
            _nextCampCheckTime = Time.time + _timeBetweenCampingChecks;

            _isCamping = (Vector3.Distance(_playerTransform.position, _campPositionOld) < _campThresholdDistance);
            _campPositionOld = _playerTransform.position;
        }
    }

    private void InitializePlayer()
    {
        _playerEntity = FindFirstObjectByType<PlayerHealth>();
        if (_playerEntity != null) _target = _playerEntity.GetComponent<ITargetable>();
        _playerTransform = _playerEntity.transform;

        _nextCampCheckTime = _timeBetweenCampingChecks + Time.time;
        _campPositionOld = _playerTransform.position;

        _playerEntity.OnDeath += OnPlayerDeath;
    }

    private void OnPlayerDeath(Entity entity)
    {
        _isDisabled = true;
    }

    [Serializable]
    private class Wave
    {
        public int _enemyCount;
        public float _timeBetweenSpawns;
    }
}
