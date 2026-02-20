using UnityEngine;

public class LevelInitializer : MonoBehaviour
{
    [SerializeField] private MapGenerator _mapGenerator;
    [SerializeField] private Spawner _enemySpawner;
    [SerializeField] private WaveGenerator _waveGenerator;
    [SerializeField] private WaveSequencer _waveSequencer;
    [SerializeField] private MapSpawner _mapSpawner;

    [SerializeField] private PlayerFacade _player;

    private void Awake()
    {
        if (_player == null) _player = GetComponent<PlayerFacade>();
    }

    private void Start()
    {
        _player.Tracker.InitializePlayer(_player.Target);
 
        _enemySpawner.Initialize(_player.Tracker, _player.Target, _mapSpawner);

        _mapGenerator.MapGenerated += _enemySpawner.OnMapGenerated;

        _mapGenerator.MapGenerated += (data) => {
            Vector3 centerPos = data.grid.CoordToWorld(data.center);

            PlayerMovement movement = _player.Movement;
            if (movement != null)
            {
                movement.Teleport(centerPos);
            }
            else
            {
                _player.Target.transform.position = centerPos;
            }
        };

        _waveSequencer.NewWave += OnNewWaveRequested;
        _waveSequencer.SpawnRequested += _enemySpawner.SpawnEnemy;
        _enemySpawner.EnemyDeath += _waveSequencer.RecordEnemyDeath;

        _player.Health.OnDeath += (e) => _enemySpawner.Disable();
    }

    private void OnNewWaveRequested(int waveNumber)
    {
        WaveData newWave = _waveSequencer.GetManualWave(waveNumber);

        if (newWave == null)
        {
            newWave = _waveGenerator.GenerateWave(waveNumber);
        }

        _waveSequencer.SetWave(newWave);
        _mapGenerator.OnNewWave(waveNumber);
    }
}
