using System.Collections;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
    [SerializeField] private float _timeBetweenChecks = .1f;
    private EnemyMovement _movement;
    private EnemyAttack _attack;
    private EnemyStats _stats;
    private ITargetable _target;
    private IHealth _selfHealth;

    private Coroutine _stateUpdateCoroutine;

    public enum State { Idle, Chasing, Attacking, Dead };
    private State _currentState;

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
                _attack.PerformAttack();
                break;
            case State.Dead:
                _movement.StopMoving();
                _movement.SetKinematic(true);
                break;
        }
    }

    private void CheckStateTransition()
    {
        if (_selfHealth .IsDead)
        {
            ChangeState(State.Dead);
            return;
        }

        if (_target == null)
        {
            Debug.Log("NO target");
            ChangeState(State.Idle);
            return;
        }

        if (_target.IsDead)
        {
            Debug.Log("Target is dead");
            ChangeState(State.Idle);
            return;
        }

        if (_attack.IsAttacking) return;

        if (_movement.DistanceToTarget <= _stats.AttackDistanceThreshold)
        {
            ChangeState(State.Attacking);
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
            if (_selfHealth.IsDead)
            {
                ChangeState(State.Dead);
                yield break;
            }

            if (_target == null)
            {
                ChangeState(State.Idle);
            }
            else
            {
                CheckStateTransition();

                if (_currentState == State.Attacking && !_attack.IsAttacking)
                    _attack.PerformAttack();
            }
            yield return new WaitForSeconds(_timeBetweenChecks);
        }
    }
}
