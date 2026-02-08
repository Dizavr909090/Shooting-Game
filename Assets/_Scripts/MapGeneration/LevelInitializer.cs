using UnityEngine;

public class LevelInitializer : MonoBehaviour
{
    [SerializeField] private MapGenerator _mapGenerator;
    [SerializeField] private Spawner _enemySpawner;
    [SerializeField] private WaveGenerator _waveGenerator;
    [SerializeField] private WaveSequencer _waveSequencer;
    [SerializeField] private PlayerTracker _playerTracker;
    [SerializeField] private MapSpawner _mapSpawner;

    private void Awake()
    {
        Entity player = FindFirstObjectByType<PlayerHealth>();
        Debug.Log($"[LevelInit] Player found: {player != null}, Tracker linked: {_playerTracker != null}");
        _playerTracker.InitializePlayer(player);
        _enemySpawner.Initialize(_playerTracker, player, _mapSpawner);

        _mapGenerator.MapGenerated += _enemySpawner.OnMapGenerated;

        _mapGenerator.MapGenerated += (data) => {
            Vector3 centerPos = data.grid.CoordToWorld(data.center);

            PlayerMovement movement = player.GetComponent<PlayerMovement>();
            if (movement != null)
            {
                movement.Teleport(centerPos);
            }
            else
            {
                player.transform.position = centerPos;
            }
        };

        _waveSequencer.NewWave += OnNewWaveRequested;
        _waveSequencer.SpawnRequested += _enemySpawner.SpawnEnemy;
        _enemySpawner.EnemyDeath += _waveSequencer.RecordEnemyDeath;

        player.OnDeath += (e) => _enemySpawner.Disable();
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
