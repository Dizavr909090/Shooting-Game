using UnityEngine;

public class LevelInitializer : MonoBehaviour
{
    [SerializeField] private MapGenerator _generator;
    [SerializeField] private Spawner _spawner;

    private void Awake()
    {
        _generator.MapGenerated += _spawner.OnMapGenerated;
    }
}
