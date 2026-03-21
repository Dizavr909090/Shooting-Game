using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private GunEventChannelSO _gunEquipChannel;

    private InputManager _input;
    private IShootable _shootable;

    private void Awake()
    {
        if (_input == null) _input = FindFirstObjectByType<InputManager>();
        if (_shootable == null) _shootable = GetComponent<IShootable>();
    }

    private void OnEnable()
    {     
        _input.ReloadPressed += _shootable.Reload;
        _gunEquipChannel.EventRaised += HandleGunChanged;
    }

    private void Update()
    {
        if (_shootable.CurrentShootMode == ShootType.Auto ||
            _shootable.CurrentShootMode == ShootType.AutoBurst ||
            _shootable.CurrentShootMode == ShootType.AutoSingle)
            HandleAutoShooting();
    }

    private void OnDisable()
    {
        _gunEquipChannel.EventRaised -= HandleGunChanged;
        _input.ReloadPressed -= _shootable.Reload;
        _input.OnShootPressed -= HandleSingleShoot;
    }

    private void HandleGunChanged(Gun newGun)
    {
        _input.OnShootPressed -= HandleSingleShoot;

        if (!newGun.IsAutomaticMode)
        {
            _input.OnShootPressed += HandleSingleShoot;
        }
    }

    private void HandleAutoShooting()
    {
        if (_input.IsShooting && _shootable.CanShoot)
        {
            _shootable.Shoot();
        }
    }

    private void HandleSingleShoot()
    {
        if (_shootable.CanShoot)
        {
            _shootable.Shoot();
        }
    }
}
