public interface IShootable
{
    ShootType CurrentShootMode { get; }

    bool CanShoot {  get; }

    void Shoot();

    void StopFiring();

    void Reload();

    void ResetAmmo();

    void CancelReload();

    void ApplyModifier(GunModifier modifier);
}
