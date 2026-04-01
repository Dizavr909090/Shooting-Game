using UnityEngine;

public class PlateModule : BaseModule
{
    [SerializeField] private Transform _weaponHold;
    [SerializeField] private float _currentAngle;
    [SerializeField] private float _targetAngle;
    [SerializeField] private float _radius;
    [SerializeField] private float _lerpSpeed;

    private Vector3 _weaponOffset = new Vector3(0, 0, 1f);

    private void Update()
    {
        _currentAngle = Mathf.MoveTowardsAngle(_currentAngle, _targetAngle, _lerpSpeed * Time.deltaTime);

        float x = Mathf.Sin(_currentAngle * Mathf.Deg2Rad);
        float z = Mathf.Cos(_currentAngle * Mathf.Deg2Rad);

        transform.localPosition = new Vector3(x * _radius, 0, z * _radius);
        transform.localEulerAngles = new Vector3(0, _currentAngle, 0);

        _weaponHold.localPosition = Vector3.MoveTowards(_weaponHold.localPosition, _weaponOffset, _lerpSpeed * Time.deltaTime);
    }

    public override void HandleCommand(ICommand command)
    {
        if (command is MovePlateCommand movePlateCommand)
        {
            if (movePlateCommand.TargetID == ID || movePlateCommand.TargetID == -1)
            {
                _targetAngle = movePlateCommand.TargetAngle;
                _weaponOffset = movePlateCommand.WeaponOffset;
            }
        }
    }
}
