using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody _myRb;

    private void Awake()
    {
        SetRigidbody();
    }

    private void Start()
    {
        PlayerFacade.Instance.Health.OnDeath += HandleDeath;
    }

    private void OnDisable()
    {
        if (PlayerFacade.Instance.Health != null)
        {
            PlayerFacade.Instance.Health.OnDeath -= HandleDeath;
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
        PlayerFacade.Instance.Movement.DisableMovement();
    }
}
