using UnityEngine;

public class TargetComponent : MonoBehaviour, ITargetable
{
    private IHealth _health;

    public Transform Transform => transform;
    public bool IsDead => _health != null && _health.IsDead;

    public void Initialize(IHealth health)
    {
        _health = health;
    }

    public float GetRadius()
    {
        if (TryGetComponent<CapsuleCollider>(out var capsule))
            return capsule.radius;

        return 0f;
    }
}
