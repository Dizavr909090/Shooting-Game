using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    [SerializeField, Range(0.1f, 2f)] private float _pathUpdateInterval = 0.25f;

    private NavMeshAgent _agent;
    private EnemyStats _stats;
    private ITargetable _target;

    private float _myCollisionRadius;
    private float _targetCollisionRadius;

    private Coroutine _pathCoroutine;

    private bool _hasTarget;

    public float DistanceToTarget
    {
        get
        {
            if (_target == null || _target.Transform == null) return float.MaxValue;

            float distance = Vector3.Distance(transform.position, _target.Transform.position);
            return distance - (_myCollisionRadius + _targetCollisionRadius);
        }
    }

    private void Awake()
    {
        _myCollisionRadius = GetComponent<CapsuleCollider>().radius;
    }

    public void Initialize(ITargetable target, EnemyStats stats, NavMeshAgent agent )
    {
        if (agent == null || stats == null)
        {
            Debug.LogError("NO COMPONENTS");
            return;
        }

        _target = target;
        _stats = stats;
        _agent = agent;

        if (_target != null) _targetCollisionRadius = _target.GetRadius();
    }

    public void StartMoving()
    {
        _hasTarget = true;

        if (_pathCoroutine != null) return;

        _pathCoroutine = StartCoroutine(UpdatePath());
    }

    public void StopMoving()
    {
        _hasTarget = false;

        if (_pathCoroutine != null)
        {
            StopCoroutine(_pathCoroutine);
            _pathCoroutine = null;
        }

        if(_agent != null && _agent.isOnNavMesh)
        {
            _agent.ResetPath();
        }
    }

    public void ResetMovement()
    {
        _hasTarget = false;

        if (_pathCoroutine != null)
        {
            StopCoroutine(_pathCoroutine);
            _pathCoroutine = null;
        }

        _agent.enabled = true;
    }

    public void UpdateTarget(ITargetable newTarget)
    {
        _target = newTarget;
        if (_target != null) _targetCollisionRadius = _target.GetRadius();
    }

    public void SetKinematic(bool isKinematic)
    {
        if (isKinematic)
        {
            _agent.enabled = false;
        }
        else
        {
            _agent.enabled = true;
        }
    }

    private IEnumerator UpdatePath()
    {
        while (_hasTarget)
        {
            if (_target == null || _target.Transform == null || _target.IsDead)
            {
                yield return new WaitForSeconds(_pathUpdateInterval);
                continue;
            }

            if (!_agent.enabled || !_agent.isOnNavMesh)
            {
                yield return new WaitForSeconds(_pathUpdateInterval);
                continue;
            }

            Vector3 dirToTarget = (_target.Transform.position - transform.position).normalized;
            Vector3 targetPosition = _target.Transform.position - dirToTarget *
                (_myCollisionRadius + _targetCollisionRadius + _stats.AttackDistanceThreshold);

            _agent.SetDestination(targetPosition);
 
            yield return new WaitForSeconds(_pathUpdateInterval);
        }

        _pathCoroutine = null;
    }
}

