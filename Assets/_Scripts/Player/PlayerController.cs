using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerFacade _player;
    [SerializeField] private Rigidbody _myRb;

    private void Awake()
    {
        if (_player == null) _player = GetComponent<PlayerFacade>();
        SetRigidbody();
    }

    private void Start()
    {
        _player.Health.OnDeath += HandleDeath;
    }

    private void OnDisable()
    {
        if (_player.Health != null)
        {
            _player.Health.OnDeath -= HandleDeath;
        }
    }

    private void SetRigidbody()
    {
        if (_myRb == null) _myRb = GetComponent<Rigidbody>();
        _myRb.interpolation = RigidbodyInterpolation.Interpolate;
        _myRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        _myRb.freezeRotation = true;

        _myRb.linearDamping = 0f;
        _myRb.angularDamping = 0f;
    }

    private void HandleDeath(IHealth _)
    {
        _player.Movement.DisableMovement();
    }
}
