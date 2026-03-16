using UnityEngine;

[CreateAssetMenu(fileName = "GunStats", menuName = "Scriptable Objects/GunStats")]
public class GunStats : ScriptableObject
{
    [field: SerializeField] public ProjectileData ProjData {  get; private set; }

    [Header("Shoot Stats")]
    [field: SerializeField, Range(1, 100)] public float MuzzleVelocityModifier { get; private set; }
    [field: SerializeField] public float FireRate { get; private set; }
    [field: SerializeField] public float BaseSpread { get; private set; }
    [field: SerializeField] public int BurstCount { get; private set; }
    [field: SerializeField] public float TimeBetweenShotsInBurst { get; private set; }
    [field: SerializeField] public float BurstCooldown { get; private set; }

    [Header("Ammo and Reload")]
    [field: SerializeField] public int MaxAmmo { get; private set; }
    [field: SerializeField] public int MagazineSize { get; private set; }
    [field: SerializeField] public float ReloadTime { get; private set; }
    [field: SerializeField] public bool IsInfiniteAmmo { get; private set; }
    [field: SerializeField] public ReloadType ReloadMode { get; private set; } 
}

[SerializeField] public enum ReloadType { Magazine, Single, None }
