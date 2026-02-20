using System;
using UnityEngine;
using UnityEngine.InputSystem;
using PlayerInput;

public class InputManager : MonoBehaviour
{
    public event Action<Vector2> OnMove;
    public event Action OnShootPressed;

    private PlayerControls _playerControls;
    
    public bool IsShooting { get; private set; }

    private void Awake()
    {
        _playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        _playerControls.Enable();
        _playerControls.Player.Move.performed += HandleMovePerformed;
        _playerControls.Player.Move.canceled += HandleMoveCanceled;

        _playerControls.Player.Shoot.performed += HandleShootPerformed;
        _playerControls.Player.Shoot.canceled += HandleShootCanceled;
    }

    void OnDisable()
    {
        if (_playerControls != null)
        {
            _playerControls.Player.Move.performed -= HandleMovePerformed;
            _playerControls.Player.Move.canceled -= HandleMoveCanceled;

            _playerControls.Player.Shoot.performed -= HandleShootPerformed;
            _playerControls.Player.Shoot.canceled -= HandleShootCanceled;

            _playerControls.Player.Disable();
        }
    }

    private void HandleMovePerformed(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        OnMove?.Invoke(input);
    }

    private void HandleMoveCanceled(InputAction.CallbackContext context)
    {
        OnMove?.Invoke(Vector2.zero);
    }

    private void HandleShootPerformed(InputAction.CallbackContext context)
    {
        IsShooting = true;
        OnShootPressed?.Invoke();
    }

    private void HandleShootCanceled(InputAction.CallbackContext context)
    {
        IsShooting = false;
    }
}
