using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private Transform _muzzleTransform;
    [SerializeField] private Projectile _projectile;
    [SerializeField] private float _msBetweenShots = 100;
    [SerializeField, Range(1,100)] private float _muzzleVelocity = 35;
    [SerializeField] private ProjectilePool _projectilePool;

    private float nextShotTime;

    private void Awake()
    {
        if (_projectilePool == null)
            _projectilePool = GetComponent<ProjectilePool>();
    }

    public void Shoot()
    {
        nextShotTime = Time.time + _msBetweenShots / 1000; //ms into s

        Projectile newProjectile = _projectilePool.GetProjectile();
        newProjectile.transform.position = _muzzleTransform.position;
        newProjectile.transform.rotation = _muzzleTransform.rotation;
        newProjectile.SetSpeedAndDirection(_muzzleVelocity, _muzzleTransform.forward);
    }

    public bool CanShoot()
    {
        return Time.time > nextShotTime;
    }
}
