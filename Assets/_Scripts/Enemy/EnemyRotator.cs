using UnityEngine;
using UnityEngine.AI;

public class EnemyRotator : MonoBehaviour
{
    private const float MIN_MAGNITUDE_TRESHOLD = 0.0001f;

    private EnemyStats _stats;
    private NavMeshAgent _agent;

    public void Initialize(EnemyStats stats, NavMeshAgent agent)
    {
        _stats = stats;
        _agent = agent;
        _agent.updateRotation = false;
    }

    public void RotateTowards(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        ApplyRotation(direction);
    }

    public void FaceDirection(Vector3 direction)
    {
        ApplyRotation(direction);
    }

    public bool IsFacingTarget(Vector3 targetPosition)
    {
        Vector3 direction = GetHorizontalDirection(targetPosition);

        if (direction.sqrMagnitude < MIN_MAGNITUDE_TRESHOLD) return true;

        float angle = Vector3.Angle(transform.forward, direction);

        return angle < _stats.RotationTreshold;
    }

    public void InstantLookAt(Vector3 targetPosition)
    {
        Vector3 direction = GetHorizontalDirection(targetPosition);

        if (direction.sqrMagnitude > MIN_MAGNITUDE_TRESHOLD)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    private void ApplyRotation(Vector3 direction)
    {
        direction.y = 0;

        if (direction.sqrMagnitude > MIN_MAGNITUDE_TRESHOLD)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                _stats.RotationSpeed * Time.deltaTime
            );
        }
    }

    private Vector3 GetHorizontalDirection(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0;
        return direction;
    }
}
