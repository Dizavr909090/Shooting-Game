using UnityEngine;

public class ChaseState : BaseState
{
    private EnemyMovement _movement;
    private EnemyStats _stats;
    private ITargetProvider _targetProvider;
    private EnemyRotator _rotator;

    public ChaseState(
        StateMachine stateMachine, 
        EnemyMovement movement, 
        EnemyStats stats,
        ITargetProvider targetProvider,
        EnemyRotator rotator) : base(stateMachine)
    {
        _movement = movement;
        _stats = stats;
        _targetProvider = targetProvider;
        _rotator = rotator;
    }

    public override void OnEnter() 
    {
        _movement.StartMoving();
    }

    public override void Update() 
    {
        if (HandleTargetLost()) return;

        _rotator.FaceDirection(_movement.Velocity);

        if (_stateMachine.DistanceToTarget <= _stats.MeleeAttackRange + _stats.DistanceTolerance)
        {
            _stateMachine.SwitchState<MeleeAttackState>();
        }

        if (_stateMachine.DistanceToTarget <= _stats.RangedAttackDistanceMax + _stats.DistanceTolerance &&
            _targetProvider.IsVisible)
        {
            _stateMachine.SwitchState<RangedAttackState>();
        }
    }

    public override void OnExit() 
    {
        
    }
}