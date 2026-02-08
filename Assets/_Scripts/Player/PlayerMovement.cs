using UnityEngine;
using UnityEngine.InputSystem;
using PlayerInput;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField, Range(1f, 20f)]
    private float _moveSpeed = 8f;
    [SerializeField, Range(0.1f, 2f)]
    private float _accelerationTime = 0.3f;
    [SerializeField, Range(0.1f, 2f)]
    private float _decelerationTime = 0.2f;
    private const float INPUT_DEADZONE = 0.1f;

    private Vector3 _targetMovementVector;
    private Vector3 _currentVelocity;
    private Vector3 _smoothDampVelocity;

    private InputManager _input;
    private Rigidbody _myRb;
    private Vector2 _rawInputVector;

    //private bool _disabled = false;

    private void Awake()
    {
        if (_myRb == null) _myRb = GetComponent<Rigidbody>();
        if (_input == null) _input = FindFirstObjectByType<InputManager>();
    }

    private void OnEnable()
    {
        _input.OnMove += HandeInput;
    }

    private void OnDisable()
    {
        _input.OnMove -= HandeInput;
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    public void Teleport(Vector3 targetPosition)
    {
        _currentVelocity = Vector3.zero;
        _smoothDampVelocity = Vector3.zero;

        _myRb.linearVelocity = Vector3.zero;
        _myRb.angularVelocity = Vector3.zero;

        _myRb.position = targetPosition;
        transform.position = targetPosition;

        _myRb.interpolation = RigidbodyInterpolation.None;
    }

    private void HandeInput(Vector2 input)
    {
        _rawInputVector = input;
    }

    private void HandleMovement()
    {
        CalculateTargetMovement();

        Vector3 smoothVelocity = CalculateSmoothVelocity();

        ApplyMovement(smoothVelocity);
    }

    private void CalculateTargetMovement()
    {
        if (_rawInputVector.magnitude < INPUT_DEADZONE)
        {
            _targetMovementVector = Vector3.zero;
            return;
        }
       
        _targetMovementVector = new Vector3(_rawInputVector.x, 0f, _rawInputVector.y).normalized;
    }

    private Vector3 CalculateSmoothVelocity()
    {
        Vector3 targetVelocity = _targetMovementVector * _moveSpeed;

        float smoothTime = _targetMovementVector.magnitude > INPUT_DEADZONE ? _accelerationTime : _decelerationTime;

        _currentVelocity = Vector3.SmoothDamp(_currentVelocity, targetVelocity, ref _smoothDampVelocity, smoothTime);

        return _currentVelocity;
    }

    private void ApplyMovement(Vector3 velocity)
    {
        _myRb.linearVelocity = new Vector3(velocity.x, _myRb.linearVelocity.y, velocity.z);
    }

    public void DisableMovement()
    {
        //_disabled = true;

        _rawInputVector = Vector3.zero;
        _targetMovementVector = Vector3.zero;
        _currentVelocity = Vector3.zero;
        _smoothDampVelocity = Vector3.zero;
        _myRb.linearVelocity = Vector3.zero;
    }
}
