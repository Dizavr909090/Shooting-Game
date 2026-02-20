using System.Collections;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
    [SerializeField] private float _timeBetweenChecks = 0.1f;
    private EnemyMovement _movement;
    private EnemyAttack _attack;
    private EnemyStats _stats;
    private ITargetable _target;
    private IHealth _selfHealth;

    private Coroutine _stateUpdateCoroutine;

    public enum State { Idle, Chasing, Attacking, Dead };
    [SerializeField] private State _currentState;

    public void Initialize(EnemyMovement movement, EnemyAttack attack, EnemyStats stats, IHealth selfHealth)
    {
        _movement = movement;
        _attack = attack;
        _stats = stats;
        _selfHealth = selfHealth;
    }

    public void StartStateMachine()
    {
        if (_stateUpdateCoroutine != null) StopCoroutine(_stateUpdateCoroutine);
        _stateUpdateCoroutine = StartCoroutine(StateUpdateTick());
    }

    public void ResetLogic()
    {
        _currentState = State.Idle;
        _target = null;

        if (_stateUpdateCoroutine != null)
            StopCoroutine(_stateUpdateCoroutine);

        _stateUpdateCoroutine = StartCoroutine(StateUpdateTick());
    }

    public void UpdateTarget(ITargetable newTarget)
    {
        _target = newTarget;
    }

    private void ChangeState(State newState)
    {
        if (_currentState == newState) return;

        if (_currentState == State.Attacking)
            _movement.SetKinematic(false);

        _currentState = newState;

        switch (_currentState)
        {
            case State.Idle:
                _movement.StopMoving();
                break;
            case State.Chasing:
                _movement.StartMoving();
                break;
            case State.Attacking:
                _movement.StopMoving();
                _movement.SetKinematic(true);
                break;
            case State.Dead:
                _movement.StopMoving();
                _movement.SetKinematic(true);
                break;
        }
    }

    private void CheckStateTransition()
    {
        if (_selfHealth.IsDead)
        {
            ChangeState(State.Dead);
            return;
        }

        if (_target == null || _target.IsDead)
        {
            Debug.Log("NO target or target is dead");
            ChangeState(State.Idle);
            return;
        }

        if (_attack.IsAttacking) return;

        if (_movement.DistanceToTarget <= _stats.AttackDistanceThreshold + _stats.AttackDistanceTolerance)
        {
            if(_currentState != State.Attacking)
                ChangeState(State.Attacking);

            if (!_attack.IsAttacking)
                _attack.PerformAttack();
        }
        else
        {
            ChangeState(State.Chasing);
        }
    }

    private IEnumerator StateUpdateTick()
    {
        yield return new WaitForEndOfFrame();

        while (_selfHealth != null)
        {
            CheckStateTransition();

            yield return new WaitForSeconds(_timeBetweenChecks);
        }
    }
}
