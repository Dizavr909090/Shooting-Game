using System.Collections;
using UnityEngine;

[RequireComponent (typeof(AmmoHandler))]
public class Gun : MonoBehaviour
{
    [SerializeField] private Transform _muzzleTransform;
    [SerializeField] private GunStats _gunStats;
    [SerializeField] private AmmoHandler _ammoHandler;

    private float _nextShotTime;
    private bool _isFiringBurst;

    public bool CanShoot => Time.time > _nextShotTime;
    public ShootType ShootMode => _gunStats.ShootMode;
    public bool IsAutomaticMode => _gunStats.IsAutomaticMode;

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
                _nextShotTime = Time.time + _gunStats.FireRate;
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

    public void StopFiring()
    {
       
    }

    private IEnumerator BurstShootCoroutine(FractionRelationsConfig.FractionType shooterFraction)
    {
        _isFiringBurst = true;

        for (int i = 0; i < _gunStats.BurstCount; i++)
        {
            if (_ammoHandler.TryConsumeAmmo())
            {
                ExecutePhysicalShot(shooterFraction);
                yield return new WaitForSeconds(_gunStats.TimeBetweenShotsInBurst);
            }
        }
        _nextShotTime = Time.time + _gunStats.BurstCooldown;

        _isFiringBurst = false;
    }

    private void ExecutePhysicalShot(FractionRelationsConfig.FractionType shooterFraction)
    {
        for (int i = 0; i < _gunStats.PelletCount; i++)
        {
            ProjectileVisual projToShoot = ProjectileProvider.Instance.GetProjectile(_gunStats.ProjData);
            projToShoot.transform.SetPositionAndRotation(
                _muzzleTransform.position,
                _muzzleTransform.rotation);

            projToShoot.PoolableComponent.ActivateTrail();

            ProjectileSimulation.Instance.AddProjectile(
                projToShoot.transform.position,
                CalculateSpread(),
                _gunStats.ProjData.BaseSpeed * _gunStats.MuzzleVelocityModifier,
                _gunStats.ProjData,
                projToShoot,
                shooterFraction
                );
        }
    }

    private Vector3 CalculateSpread()
    {
        Vector2 randomCircle = Random.insideUnitCircle;

        float spreadIntensity = _gunStats.BaseSpread;

        Vector3 spreadDirection = _muzzleTransform.forward
            + (_muzzleTransform.right * randomCircle.x * spreadIntensity)
            + (_muzzleTransform.forward);

        spreadDirection.Normalize();

        return spreadDirection;
    }
}
