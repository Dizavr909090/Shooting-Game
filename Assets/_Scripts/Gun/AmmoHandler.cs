using System;
using System.Collections;
using UnityEngine;

public class AmmoHandler : MonoBehaviour
{
    public event Action ReloadStart;
    public event Action ReloadEnd;
    public event Action AmmoChanged;

    private int _maxAmmo;
    private int _magazineSize;
    private float _reloadTime;
    private bool _isInfiniteAmmo;
    private ReloadType _reloadMod;

    private int _currentAmmo;
    private int _currentAmmoInMagazine;

    private Coroutine _reloadCoroutine;

    public int CurrentAmmo => _currentAmmo;
    public int CurrentAmmoInMagazine => _currentAmmoInMagazine;
    public bool IsReloading {  get; private set; }
    
    public void Initialize(
        float reloadTime, 
        int maxAmmo, 
        int magazineSize,
        bool isInfiniteAmmo, 
        ReloadType reloadMod)
    {
        _reloadTime = reloadTime;
        _maxAmmo = maxAmmo;
        _magazineSize = magazineSize;
        _isInfiniteAmmo = isInfiniteAmmo;
        _reloadMod = reloadMod;

        _currentAmmo = maxAmmo;
        _currentAmmoInMagazine = magazineSize;
    }

    public bool TryConsumeAmmo()
    {
        if (_isInfiniteAmmo) return true;

        if (_currentAmmo == 0 && _currentAmmoInMagazine == 0) return false;

        if (IsReloading && _reloadMod != ReloadType.Single) return false;

        if (_currentAmmoInMagazine == 0)
        {
            StartReload();
            return false;
        }

        if (IsReloading && 
            _currentAmmoInMagazine > 0 &&
            _reloadMod == ReloadType.Single)
            CancelReload();

        _currentAmmoInMagazine--;
        AmmoChanged?.Invoke();

        return true;
    }

    public void StartReload()
    {
        if (!CanReload()) return;

        CancelReload();
        _reloadCoroutine = StartCoroutine(Reload());
        ReloadStart?.Invoke();
    }

    public bool CanReload()
    {
        if (_currentAmmoInMagazine == _magazineSize ||
            _currentAmmo <= 0 ||
            _isInfiniteAmmo ||
             IsReloading ||
            _reloadMod == ReloadType.None) return false;

        return true;
    }   

    public void CancelReload()
    {       
        if (_reloadCoroutine != null)
        {
            StopCoroutine(_reloadCoroutine);
            _reloadCoroutine= null;
        }

        IsReloading = false;
    }

    private IEnumerator Reload()
    {
        if (CanReload())
        {
            IsReloading = true;

            if (_reloadMod == ReloadType.Single)
            {
                while (_currentAmmoInMagazine != _magazineSize)
                {
                    yield return new WaitForSeconds(_reloadTime);

                    _currentAmmo--;
                    _currentAmmoInMagazine++;
                    AmmoChanged?.Invoke();
                }
            }

            if (_reloadMod == ReloadType.Magazine)
            {
                yield return new WaitForSeconds(_reloadTime);

                int neededAmmo = _magazineSize - _currentAmmoInMagazine;
                int availableAmmo = Mathf.Min(neededAmmo, _currentAmmo);
                _currentAmmo -= availableAmmo;
                _currentAmmoInMagazine += availableAmmo;
            }
        }

        CancelReload();
        ReloadEnd?.Invoke();
        AmmoChanged?.Invoke();
    }
}
