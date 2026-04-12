using UnityEngine;

public class PlateModule : BaseModule
{
    [SerializeField] private Transform _weaponHold;
    [SerializeField] private IShootable _shootable;
    [SerializeField] private float _currentAngle;
    [SerializeField] private float _targetAngle;
    [SerializeField] private float _radius;
    [SerializeField] private float _lerpSpeed;

    private Vector3 _weaponOffset = new Vector3(0, 0, 1f);
    private bool _isFiring;

    public override bool IsAtTarget =>
    Mathf.Abs(Mathf.DeltaAngle(_currentAngle, _targetAngle)) < 0.1f &&
    Vector3.Distance(_weaponHold.localPosition, _weaponOffset) < 0.01f;

    private void Awake()
    {
        _shootable ??= GetComponent<IShootable>();
    }

    private void Update()
    {
        _currentAngle = Mathf.MoveTowardsAngle(_currentAngle, _targetAngle, _lerpSpeed * Time.deltaTime);

        float x = Mathf.Sin(_currentAngle * Mathf.Deg2Rad);
        float z = Mathf.Cos(_currentAngle * Mathf.Deg2Rad);

        transform.localPosition = new Vector3(x * _radius, 0, z * _radius);
        transform.localEulerAngles = new Vector3(0, _currentAngle, 0);

        _weaponHold.localPosition = Vector3.MoveTowards(_weaponHold.localPosition, _weaponOffset, _lerpSpeed * Time.deltaTime);

        if (_isFiring)
        {
            if (_shootable.CanShoot)
            {
                _shootable?.Shoot();
            }
        }       
    }

    public override void HandleCommand(ICommand command)
    {
        HandleMovePlateCommand(command);
        HandleShootCommand(command);
    }

    private void HandleMovePlateCommand(ICommand command)
    {
        if (command is MovePlateCommand movePlateCommand)
        {
            if (movePlateCommand.TargetID == ModuleID || movePlateCommand.TargetID == -1)
            {
                Debug.Log($"[Plate {ModuleID}] Moving to Angle: {movePlateCommand.TargetAngle}");
                _targetAngle = movePlateCommand.TargetAngle;
                _weaponOffset = movePlateCommand.WeaponOffset;
            }
        }
    }

    private void HandleShootCommand(ICommand command)
    {
        if (command is ShootCommand fireCommand)
        {
            if (fireCommand.TargetID == ModuleID || fireCommand.TargetID == -1)
            {
                _isFiring = fireCommand.IsFiring;
                _shootable?.ApplyModifier(fireCommand.GunModifier);

                if (!_isFiring) _shootable?.StopFiring();
            }
        }
    }
}
