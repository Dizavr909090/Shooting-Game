using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private string _currentStateName;

    [SerializeField] private float _timeBetweenChecks = 0.02f;
    [SerializeField] private GunEventChannelSO _gunEquipChannel;

    private EnemyMovement _enemyMovement;
    private EnemyAttack _enemyAttack;
    private EnemyStats _enemyStats;
    private EnemyRotator _enemyRotator;
    private IHealth _enemyHealth;
    private ITargetProvider _targetProvider;
    private IShootable _shootable;
    private IAmmoProvider _ammoProvider;

    private float _distanceToTarget;

    private Dictionary<Type, IState> _states;
    private IState _currentState;
    private Coroutine _stateUpdateCoroutine;

    public float DistanceToTarget => _distanceToTarget;
    public ITargetable CurrentTarget => _targetProvider?.Target;
    public IAmmoProvider AmmoProvider => _ammoProvider;
    public bool IsTargetVisible => _targetProvider != null && _targetProvider.IsVisible;

    private void OnEnable()
    {
        if (_gunEquipChannel != null)
            _gunEquipChannel.EventRaised += HandleGunEquipped;
    }

    private void OnDisable()
    {
        if (_gunEquipChannel != null)
            _gunEquipChannel.EventRaised += HandleGunEquipped;
    }

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
        _currentStateName = typeof(T).Name;
        _currentState?.OnEnter();
    }

    public void Initialize(
        EnemyMovement movement, 
        EnemyAttack attack, 
        EnemyStats stats, 
        IHealth health, 
        ITargetProvider targetProvider,
        IShootable shootable,
        IAmmoProvider ammoProvider,
        EnemyRotator rotator)
    {
        _enemyMovement = movement;
        _enemyAttack = attack;
        _enemyStats = stats;
        _enemyHealth = health;
        _targetProvider = targetProvider;
        _shootable = shootable;
        _ammoProvider = ammoProvider;
        _enemyRotator = rotator;

        _states = new Dictionary<Type, IState>()
        {
            {typeof(IdleState), new IdleState(this, _enemyMovement) },
            {typeof(ChaseState), new ChaseState(this, _enemyMovement, _enemyStats, _targetProvider, _enemyRotator) },
            {typeof(MeleeAttackState), new MeleeAttackState(this, _enemyMovement, _enemyStats, _enemyAttack) },
            {typeof(RangedAttackState), new RangedAttackState(this, _enemyMovement, _enemyStats, _shootable, _enemyRotator ) },
            {typeof(ReloadState), new ReloadState(this, _enemyMovement, _shootable) },
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
            else if (_currentState != _states[typeof(ReloadState)] &&
                 _currentState != _states[typeof(DeadState)] &&
                 CheckReloadCondition())
            {
                SwitchState<ReloadState>();
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

    private bool CheckReloadCondition()
    {
        return _ammoProvider != null && _ammoProvider.NeedsReload;
    }

    private void HandleGunEquipped(Gun equippedGun)
    {
        _ammoProvider = equippedGun.AmmoProvider;
    }
}
