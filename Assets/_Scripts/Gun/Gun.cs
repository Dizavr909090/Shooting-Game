using UnityEngine;
using DG.Tweening;

public class Gun : MonoBehaviour, IShootable
{
    [SerializeField] private Transform _muzzleTransform;
    [SerializeField] private GunStats _gunStats;

    private float nextShotTime;

    public bool CanShoot => Time.time > nextShotTime;

    public void Shoot()
    {
        nextShotTime = Time.time + _gunStats.TimeBetweenShots;

        ProjectileVisual projToShoot = ProjectileProvider.Instance.GetProjectile(_gunStats.ProjData);
        projToShoot.transform.position = _muzzleTransform.position;
        projToShoot.transform.rotation = _muzzleTransform.rotation;

        projToShoot.PoolableComponent.ActivateTrail();

        ProjectileSimulation.Instance.AddProjectile(
            projToShoot.transform.position,
            CalculateSpread(),
            _gunStats.ProjData.BaseSpeed * _gunStats.MuzzleVelocityModifier,
            _gunStats.ProjData,
            projToShoot
            );

        Camera.main.transform.DOShakePosition(0.05f, 0.05f, 1, 90, false, true);
        transform.DOLocalMoveZ(-0.1f, 0.05f).SetRelative().SetLoops(2, LoopType.Yoyo);
        transform.DOPunchRotation(new Vector3(-20, 0, 0), 0.1f);
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
