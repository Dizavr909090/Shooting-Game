using UnityEngine;

public class LevelInitializer : MonoBehaviour
{
    [SerializeField] private MapGenerator _mapGenerator;
    [SerializeField] private Spawner _enemySpawner;
    [SerializeField] private WaveGenerator _waveGenerator;
    [SerializeField] private WaveSequencer _waveSequencer;

    private void Start()
    {
        PlayerFacade.Instance.Tracker.InitializePlayer(PlayerFacade.Instance.Target);
 
        _enemySpawner.Initialize(PlayerFacade.Instance.Tracker, PlayerFacade.Instance.Target);

        _waveSequencer.NewWave += OnNewWaveRequested;

        _mapGenerator.MapGenerated += _enemySpawner.OnMapGenerated;

        _mapGenerator.MapGenerated += (data) => {

            if (PlayerFacade.Instance.Movement != null)
            {
                PlayerFacade.Instance.Movement.Teleport(data.WorldCenter);
            }
            else
            {
                PlayerFacade.Instance.Target.transform.position = data.WorldCenter;
            }
        };

        
        _waveSequencer.SpawnRequested += _enemySpawner.SpawnEnemy;
        _enemySpawner.EnemyDeath += _waveSequencer.RecordEnemyDeath;

        PlayerFacade.Instance.Health.OnDeath += (e) => _enemySpawner.Disable();
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
