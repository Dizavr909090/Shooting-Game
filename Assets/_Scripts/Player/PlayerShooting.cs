using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    private InputManager _input;
    private GunController _gunController;

    private void Awake()
    {
        if (_input == null) _input = FindFirstObjectByType<InputManager>();
        if (_gunController == null) _gunController = GetComponent<GunController>();
    }

    private void OnEnable()
    {
        _input = FindFirstObjectByType<InputManager>();
    }

    private void Update()
    {       
        if (_input.IsShooting)
        {
            _gunController.TryShoot();
        } 
    }
}
