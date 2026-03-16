using UnityEngine;

public class WeaponHUD : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI _ammoText;
    [SerializeField] private GunEventChannelSO _gunEquipChannel;
    private AmmoHandler _currentAmmoHandler;

    private void OnEnable()
    {
        _gunEquipChannel.EventRaised += HandleGunChanged;

        if (_gunEquipChannel.CurrentGun != null)
        {
            HandleGunChanged(_gunEquipChannel.CurrentGun);
        }
    }

    private void OnDisable()
    {
        _gunEquipChannel.EventRaised -= HandleGunChanged;
    }

    private void HandleGunChanged(Gun gun)
    {
        if (_currentAmmoHandler != null)
        {
            _currentAmmoHandler.AmmoChanged -= UpdateAmmoDisplay;
        }

        _currentAmmoHandler = gun.GetComponent<AmmoHandler>();

        if (_currentAmmoHandler != null)
        {
            _currentAmmoHandler.AmmoChanged += UpdateAmmoDisplay;

            UpdateAmmoDisplay();
        }

    }

    private void UpdateAmmoDisplay()
    {
        string currentAmmo = _currentAmmoHandler.CurrentAmmo.ToString();
        string magazineAmmo = _currentAmmoHandler.CurrentAmmoInMagazine.ToString();

        _ammoText.text = $"{magazineAmmo} / {currentAmmo}";
    }
}
