using UnityEngine;

[CreateAssetMenu(fileName = "GunStats", menuName = "Scriptable Objects/GunStats")]
public class GunStats : ScriptableObject
{
    [field: SerializeField] public ProjectileData ProjData {  get; private set; }

    [Header("Gun Stats")]
    [field: SerializeField, Range(1, 100)] public float MuzzleVelocityModifier { get; private set; } = 1;
    [field: SerializeField] public float TimeBetweenShots { get; private set; } = 0.1f;
    [field: SerializeField] public float BaseSpread { get; private set; }
}
