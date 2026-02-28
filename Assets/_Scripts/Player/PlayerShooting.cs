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
    }

    private void Update()
    {       
        if (_input.IsShooting)
        {
            if (_shootable.CanShoot)
            {
                _shootable.Shoot();
            }
            
            Camera.main.transform.DOShakePosition(0.03f, 0.03f, 1, 90, false, true);
        } 
    }
}
