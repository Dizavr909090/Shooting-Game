public interface IShootable
{
    bool CanShoot {  get; }

    void Shoot();
    void StopFiring();
}
