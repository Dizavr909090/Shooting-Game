using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileData", menuName = "Scriptable Objects/ProjectileData")]
public class ProjectileData : ScriptableObject
{
    [field: SerializeField] public ProjectileVisual projPrefab { get; private set; }

    [Header("Projectile stats")]
    [field: SerializeField] public float Damage { get; private set; } = 10;
    [field: SerializeField] public float BaseSpeed { get; private set; } = 35;
    [field: SerializeField] public float LifeTime { get; private set; } = 2f;
}
