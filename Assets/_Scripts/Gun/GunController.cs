using System;
using UnityEngine;

public class GunController : MonoBehaviour, IShootable
{
    [SerializeField] private Gun _startingGun;
    [SerializeField] private Transform _weaponHold;
    [SerializeField] private GunEventChannelSO _gunEquipChannel;
    
    private FractionRelationsConfig.FractionType _shooterFractionType;
    private Gun _equippedGun;

    public ShootType CurrentShootMode => _equippedGun.ShootMode;
    public bool CanShoot => _equippedGun != null && _equippedGun.CanShoot;

    private void OnEnable()
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

    public void Reload()
    {
        _equippedGun.ForceReload();
    }

    public void ResetAmmo()
    {
        _equippedGun.ForceResetAmmo();
    }

    public void CancelReload()
    {
        _equippedGun.ForceCancelReload();
    }

    public void EquipGun(Gun gunToEquip)
    {
        if (_equippedGun != null && _equippedGun.name.Contains(gunToEquip.name))
        {
            _equippedGun.ForceResetAmmo();
            return;
        }

        if (_equippedGun != null) Destroy(_equippedGun.gameObject);

        _equippedGun = Instantiate(gunToEquip, _weaponHold.position, _weaponHold.rotation);
        _equippedGun.transform.parent = _weaponHold;
        _shooterFractionType = _equippedGun.GetComponentInParent<IFractionProvider>()?.FractionType ?? 
            FractionRelationsConfig.FractionType.Neutral;

        _gunEquipChannel.RaiseEvent(_equippedGun);
    }

    private void SelectStartingGun()
    {
        if (_startingGun != null)
        {
            EquipGun(_startingGun);
            _startingGun.ForceResetAmmo();
        }
    }
}
