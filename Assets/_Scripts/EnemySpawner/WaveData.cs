using UnityEngine;

[CreateAssetMenu(fileName = "WaveData", menuName = "Waves/WaveData")]
public class WaveData : ScriptableObject
{
    [field: SerializeField] public string WaveName { get; private set; } = "Wave";
    [field: SerializeField] public int EnemyCount { get; private set; } = 10;
    [field: SerializeField] public float TimeBetweenSpawns { get; private set; } = 1f;

    public void SetName(string name) => WaveName = name;
    public void SetEnemyCount(int count) => EnemyCount = count;
    public void SetSpawnDelay(float delay) => TimeBetweenSpawns = delay;

}
