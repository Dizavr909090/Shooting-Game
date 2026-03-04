using UnityEngine;

public class GunController : MonoBehaviour, IShootable
{
    [SerializeField] private Transform _weaponHold;
    [SerializeField] private Gun _startingGun;
    
    private FractionRelationsConfig.FractionType _shooterFractionType;
    private Gun _equippedGun;

    private enum WhoIsUsing { Player, Enemy }

    public bool CanShoot => _equippedGun != null && _equippedGun.CanShoot;

    private void Start()
    {
        SelectStartingGun();
    }

    public void Shoot()
    {
        _equippedGun?.Shoot(_shooterFractionType);
    }

    public void StopFiring()
    {
        _equippedGun?.StopFiring();
    }

    public void EquipGun(Gun gunToEquip)
    {
        if (_equippedGun != null)
        {
            Destroy(_equippedGun.gameObject);
        }
        _equippedGun = Instantiate(gunToEquip, _weaponHold.position, _weaponHold.rotation);
        _equippedGun.transform.parent = _weaponHold;
        _shooterFractionType = _equippedGun.GetComponentInParent<IFractionProvider>()?.FractionType ?? 
            FractionRelationsConfig.FractionType.Neutral;
    }

    private void SelectStartingGun()
    {
        if (_startingGun != null)
        {
            EquipGun(_startingGun);
        }
    }
}
