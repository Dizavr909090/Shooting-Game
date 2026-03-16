using DG.Tweening;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    private InputManager _input;
    private IShootable _shootable;

    private void Awake()
    {
        if (_input == null) _input = FindFirstObjectByType<InputManager>();
        if (_shootable == null) _shootable = GetComponent<IShootable>();
    }

    private void OnEnable()
    {
        _input = FindFirstObjectByType<InputManager>();
        _input.ReloadPressed += _shootable.Reload;
    }

    private void Update()
    {
        HandleShooting();
    }

    private void OnDisable()
    {
        _input.ReloadPressed -= _shootable.Reload;
    }

    private void HandleShooting()
    {
        if (_input.IsShooting)
        {
            if (_shootable.CanShoot)
            {
                _shootable.Shoot();              
            }
        }
    }
}
