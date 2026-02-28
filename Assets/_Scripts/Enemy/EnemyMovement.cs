using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    [SerializeField, Range(0.1f, 2f)] private float _pathUpdateInterval = 0.2f;

    private NavMeshAgent _agent;
    private ITargetProvider _targetProvider;
    private Coroutine _pathCoroutine;
    private bool _isMoving;

    public Vector3 Velocity => _agent.velocity;

    public void Initialize(ITargetProvider targetProvider, NavMeshAgent agent )
    {

        _targetProvider = targetProvider;
        _agent = agent;
    }

    public void StartMoving()
    {
        if (_isMoving) return;

        _isMoving = true;
        _pathCoroutine = StartCoroutine(UpdatePath());
    }

    public void StopMoving()
    {
        _isMoving = false;

        if (_pathCoroutine != null)
        {
            StopCoroutine(_pathCoroutine);
            _pathCoroutine = null;
        }

        if(_agent.isOnNavMesh) _agent.ResetPath();
    }

    public void ResetMovement()
    {
        _isMoving = false;

        if (_pathCoroutine != null)
        {
            StopCoroutine(_pathCoroutine);
            _pathCoroutine = null;
        }

        _agent.enabled = true;
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

    public void MoveTo(Vector3 targetPosition)
    {
        if (_agent.isOnNavMesh && _agent.enabled)
        {
            _agent.SetDestination(targetPosition);
        }
    }

    private IEnumerator UpdatePath()
    {
        while (_isMoving)
        {
            var target = _targetProvider.Target;

            if (target != null && !target.IsDead && _agent.isOnNavMesh)
                _agent.SetDestination(target.Transform.position);

            yield return new WaitForSeconds(_pathUpdateInterval);
        }

        _pathCoroutine = null;
    }
}

