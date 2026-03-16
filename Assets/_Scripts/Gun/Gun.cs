using UnityEngine;

[RequireComponent (typeof(AmmoHandler))]
public class Gun : MonoBehaviour
{
    [SerializeField] private Transform _muzzleTransform;
    [SerializeField] private GunStats _gunStats;
    [SerializeField] private AmmoHandler _ammoHandler;

    private float nextShotTime;

    public bool CanShoot => Time.time > nextShotTime;

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
        if (_ammoHandler.TryConsumeAmmo())
        {
            nextShotTime = Time.time + _gunStats.FireRate;

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

    public void ForceReload()
    {
        _ammoHandler.StartReload();
    }

    public void StopFiring()
    {
       
    }

    private Vector3 CalculateSpread()
    {
        Vector2 randomCircle = Random.insideUnitCircle;

        float spreadIntensity = _gunStats.BaseSpread;

        Vector3 spreadDirection = _muzzleTransform.forward
            + (_muzzleTransform.right * randomCircle.x * spreadIntensity)
            + (_muzzleTransform.up * randomCircle.y * spreadIntensity);

        spreadDirection.Normalize();

        return spreadDirection;
    }
}
