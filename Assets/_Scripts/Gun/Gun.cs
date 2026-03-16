using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private Transform _muzzleTransform;
    [SerializeField] private GunStats _gunStats;

    private float nextShotTime;

    public bool CanShoot => Time.time > nextShotTime;

    public void Shoot(FractionRelationsConfig.FractionType shooterFraction)
    {
        nextShotTime = Time.time + _gunStats.FireRate;

        ProjectileVisual projToShoot = ProjectileProvider.Instance.GetProjectile(_gunStats.ProjData);
        projToShoot.transform.position = _muzzleTransform.position;
        projToShoot.transform.rotation = _muzzleTransform.rotation;

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

    public void StopFiring()
    {
        throw new System.NotImplementedException();
    }
}
