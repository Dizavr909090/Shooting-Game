using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    [SerializeField] private float _timeBetweenChecks = 0.02f;

    private EnemyMovement _enemyMovement;
    private EnemyAttack _enemyAttack;
    private EnemyStats _enemyStats;
    private EnemyRotator _enemyRotator;
    private IHealth _enemyHealth;
    private ITargetProvider _targetProvider;
    private IShootable _shootable;

    private float _distanceToTarget;
    

    private Dictionary<Type, IState> _states;
    [SerializeField] private IState _currentState;
    private Coroutine _stateUpdateCoroutine;

    public float DistanceToTarget => _distanceToTarget;
    [SerializeField] public ITargetable CurrentTarget => _targetProvider?.Target;

    private void Update()
    {
        _currentState?.Update();
    }

    public void ResetLogic()
    {
        SwitchState<IdleState>();

        if (_stateUpdateCoroutine != null)
            StopCoroutine(_stateUpdateCoroutine);

        _stateUpdateCoroutine = StartCoroutine(StateUpdateTick());
    }

    public void SwitchState<T>() where T : IState
    {
        var newState = _states[typeof(T)];
        if (newState == _currentState) return;

        _currentState?.OnExit();
        _currentState = _states[typeof(T)];
        _currentState?.OnEnter();
    }

    public void Initialize(
        EnemyMovement movement, 
        EnemyAttack attack, 
        EnemyStats stats, 
        IHealth health, 
        ITargetProvider targetProvider,
        IShootable shootable,
        EnemyRotator rotator)
    {
        _enemyMovement = movement;
        _enemyAttack = attack;
        _enemyStats = stats;
        _enemyHealth = health;
        _targetProvider = targetProvider;
        _shootable = shootable;
        _enemyRotator = rotator;

        _states = new Dictionary<Type, IState>()
        {
            {typeof(IdleState), new IdleState(this, _enemyMovement) },
            {typeof(ChaseState), new ChaseState(this, _enemyMovement, _enemyStats, _targetProvider, _enemyRotator) },
            {typeof(MeleeAttackState), new MeleeAttackState(this, _enemyMovement, _enemyStats, _enemyAttack) },
            {typeof(RangedAttackState), new RangedAttackState(this, _enemyMovement, _enemyStats, _shootable, _enemyRotator ) },
            {typeof(DeadState), new DeadState(this, _enemyMovement) }
        };

        StartStateMachine();

        _currentState = _states[typeof(IdleState)];
        _currentState.OnEnter();
    }

    private void StartStateMachine()
    {
        if (_stateUpdateCoroutine != null) StopCoroutine(_stateUpdateCoroutine);
        _stateUpdateCoroutine = StartCoroutine(StateUpdateTick());
    }

    private IEnumerator StateUpdateTick()
    {
        yield return new WaitForEndOfFrame();

        while (gameObject.activeSelf)
        {
            UpdateCommonData();

            if (_enemyHealth.IsDead && _currentState != _states[typeof(DeadState)])
            {
                SwitchState<DeadState>();
            }

            yield return new WaitForSeconds(_timeBetweenChecks);
        }
    }

    private void UpdateCommonData()
    {
        if (CurrentTarget != null)
        {
            _distanceToTarget = Vector3.Distance(transform.position, CurrentTarget.Transform.position);
        }
        else
        {
            _distanceToTarget = float.MaxValue;
        }
    }
}
