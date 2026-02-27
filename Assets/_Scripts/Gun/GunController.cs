using UnityEngine;

public class GunController : MonoBehaviour
{
    [SerializeField]
    private Transform _weaponHold;
    [SerializeField]
    private Gun _startingGun;
    private Gun _equippedGun;

    private void Start()
    {
        SelectStartingGun();
    }

    public void TryShoot()
    {
        if (_equippedGun != null && _equippedGun.CanShoot)
        {
            _equippedGun.Shoot();
        }
    }

    public void EquipGun(Gun gunToEquip)
    {
        if (_equippedGun != null)
        {
            Destroy(_equippedGun.gameObject);
        }
        _equippedGun = Instantiate(gunToEquip, _weaponHold.position, _weaponHold.rotation);
        _equippedGun.transform.parent = _weaponHold;
    }

    private void SelectStartingGun()
    {
        if (_startingGun != null)
        {
            EquipGun(_startingGun);
        }
    }
}
