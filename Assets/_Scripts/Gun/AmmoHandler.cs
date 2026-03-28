using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

public class AmmoHandler : MonoBehaviour, IAmmoProvider
{
    public event Action ReloadStart;
    public event Action ReloadEnd;
    public event Action AmmoChanged;
    public event Action<float> ReloadProgressChanged;

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
    public bool IsMagazineEmpty => _currentAmmoInMagazine <= 0;
    public bool HasAnyAmmo => (_currentAmmoInMagazine > 0 || _currentAmmo > 0);
    public bool HasReserveAmmo => _currentAmmo > 0;
    public bool NeedsReload => IsMagazineEmpty && HasReserveAmmo && !IsReloading;

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

    public void ResetAmmo()
    {
        _currentAmmo = _maxAmmo;
        _currentAmmoInMagazine = _magazineSize;
    }

    private IEnumerator Reload()
    {
        if (!CanReload()) yield break;

        IsReloading = true;

        if (_reloadMod == ReloadType.Magazine)
        {
            float timer = 0;
            
            while (timer < _reloadTime)
            {
                timer += Time.deltaTime;
                float progress = timer / _reloadTime;
                ReloadProgressChanged?.Invoke(progress);

                yield return null;
            }

            int neededAmmo = _magazineSize - _currentAmmoInMagazine;
            int availableAmmo = Mathf.Min(neededAmmo, _currentAmmo);
            _currentAmmo -= availableAmmo;
            _currentAmmoInMagazine += availableAmmo;
        }
        else if (_reloadMod == ReloadType.Single)
        {
            while (_currentAmmoInMagazine < _magazineSize && _currentAmmo > 0)
            {
                float timer = 0;

                while (timer < _reloadTime)
                {
                    timer += Time.deltaTime;
                    float progress = timer / _reloadTime;
                    ReloadProgressChanged?.Invoke(progress);

                    yield return null;
                }

                _currentAmmo--;
                _currentAmmoInMagazine++;
                AmmoChanged?.Invoke();
            }
        }

        ReloadProgressChanged?.Invoke(0f);
        IsReloading = false;
        _reloadCoroutine = null;
        ReloadEnd?.Invoke();
        AmmoChanged?.Invoke();
    }
}
