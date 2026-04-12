using System.Collections;
using UnityEngine;

[RequireComponent (typeof(AmmoHandler))]
public class Gun : MonoBehaviour
{
    [SerializeField] private Transform _muzzleTransform;
    [SerializeField] private GunStats _gunStats;
    [SerializeField] private AmmoHandler _ammoHandler;

    private GunModifier _currentModifier = GunModifier.Default;
    private float _nextShotTime;
    private bool _isFiringBurst;

    public bool CanShoot => Time.time > _nextShotTime;
    public ShootType ShootMode => _gunStats.ShootMode;
    public bool IsAutomaticMode => _gunStats.IsAutomaticMode;
    public bool NeedsReload => _ammoHandler.IsMagazineEmpty;
    public IAmmoProvider AmmoProvider => _ammoHandler;

    private void Awake()
    {
        _ammoHandler.Initialize(
            _gunStats.ReloadTime,
            _gunStats.MaxAmmo,
            _gunStats.MagazineSize,
            _gunStats.IsInfiniteAmmo,
            _gunStats.ReloadMode);
    }

    public void Shoot(FractionRelationsConfig.FractionType shooterFraction)
    {
        if (ShootMode == ShootType.Auto || ShootMode == ShootType.Single || ShootMode == ShootType.AutoSingle)
        {
            if (_ammoHandler.TryConsumeAmmo())
            {
                ExecutePhysicalShot(shooterFraction);
                _nextShotTime = Time.time + _gunStats.FireRate / Mathf.Max(0.01f, _currentModifier.FireRateModifier); ;
            }
        }
        else if (ShootMode == ShootType.Burst || ShootMode == ShootType.AutoBurst)
        {
            if (_isFiringBurst) return;

            StartCoroutine(BurstShootCoroutine(shooterFraction));
        }
    }

    public void ForceReload()
    {
        _ammoHandler.StartReload();
    }

    public void ForceCancelReload()
    {
        _ammoHandler.CanReload();
    }

    public void ForceResetAmmo()
    {
        _ammoHandler.ResetAmmo();
    }

    public void StopFiring()
    {
        _isFiringBurst = false;
        ApplyModifier(GunModifier.Default);
    }

    public void ApplyModifier(GunModifier modifier)
    {
        _currentModifier = modifier;
    }

    private IEnumerator BurstShootCoroutine(FractionRelationsConfig.FractionType shooterFraction)
    {
        _isFiringBurst = true;

        for (int i = 0; i < _gunStats.BurstCount; i++)
        {
            if (_ammoHandler.TryConsumeAmmo())
            {
                ExecutePhysicalShot(shooterFraction);
                yield return new WaitForSeconds(_gunStats.TimeBetweenShotsInBurst * _currentModifier.FireRateModifier);
            }
        }
        _nextShotTime = Time.time + _gunStats.BurstCooldown / Mathf.Max(0.01f, _currentModifier.FireRateModifier);

        _isFiringBurst = false;
    }

    private void ExecutePhysicalShot(FractionRelationsConfig.FractionType shooterFraction)
    {
        ProjectileData dataToUse = _currentModifier.ProjectileOverride != null
            ? _currentModifier.ProjectileOverride
            : _gunStats.ProjData;

        for (int i = 0; i < _gunStats.PelletCount; i++)
        {
            ProjectileVisual projToShoot = ProjectileProvider.Instance.GetProjectile(dataToUse);
            projToShoot.transform.SetPositionAndRotation(
                _muzzleTransform.position,
                _muzzleTransform.rotation);

            projToShoot.PoolableComponent.ActivateTrail();
            projToShoot.transform.localScale = dataToUse.projPrefab.transform.localScale * _currentModifier.ScaleModifier;

            ProjectileSimulation.Instance.AddProjectile(
                projToShoot.transform.position,
                CalculateSpread(),
                _gunStats.ProjData.BaseSpeed * _gunStats.MuzzleVelocityModifier * _currentModifier.SpeedModifier,
                _gunStats.ProjData,
                projToShoot,
                shooterFraction,
                _currentModifier.DamageModifier
                );
        }
    }

    private Vector3 CalculateSpread()
    {
        Vector2 randomCircle = Random.insideUnitCircle;

        float spreadIntensity = _gunStats.BaseSpread * _currentModifier.SpreadModifier;

        Vector3 spreadDirection = _muzzleTransform.forward
            + (_muzzleTransform.right * randomCircle.x * spreadIntensity)
            + (_muzzleTransform.forward);

        spreadDirection.Normalize();

        return spreadDirection;
    }
}
