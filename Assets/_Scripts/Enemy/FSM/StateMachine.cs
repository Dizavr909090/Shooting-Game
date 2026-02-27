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
    private IHealth _enemyHealth;

    private Dictionary<Type, IState> _states;
    private IState _currentState;
    private Coroutine _stateUpdateCoroutine;

    public ITargetable CurrentTarget { get; private set; }

    public void UpdateTarget(ITargetable newTarget)
    {
        CurrentTarget = newTarget;
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

    public void Initialize(EnemyMovement movement, EnemyAttack attack, EnemyStats stats, IHealth health)
    {
        _enemyMovement = movement;
        _enemyAttack = attack;
        _enemyStats = stats;
        _enemyHealth = health;

        _states = new Dictionary<Type, IState>()
        {
            {typeof(IdleState), new IdleState(this, _enemyMovement) },
            {typeof(ChaseState), new ChaseState(this, _enemyMovement, _enemyStats) },
            {typeof(MeleeAttackState), new MeleeAttackState(this, _enemyMovement, _enemyStats, _enemyAttack) },
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
            if (_enemyHealth.IsDead && _currentState != _states[typeof(DeadState)])
            {
                SwitchState<DeadState>();
            }

            _currentState?.Update();

            yield return new WaitForSeconds(_timeBetweenChecks);
        }
    }
}
