using UnityEngine;

public class ChaseState : BaseState
{
    private EnemyMovement _movement;
    private EnemyStats _stats;

    public ChaseState(
        StateMachine stateMachine, 
        EnemyMovement movement, 
        EnemyStats stats) : base(stateMachine)
    {
        _movement = movement;
        _stats = stats;
    }

    public override void OnEnter() 
    {
        _movement.StartMoving();
    }

    public override void Update() 
    {
        if (HandleTargetLost()) return;
        
        if (_movement.DistanceToTarget <= _stats.AttackDistanceThreshold + _stats.AttackDistanceTolerance)
        {
            _stateMachine.SwitchState<MeleeAttackState>();
        }

        
    }

    public override void OnExit() 
    {
        
    }

    private bool HandleTargetLost()
    {
        if (_stateMachine.CurrentTarget == null || _stateMachine.CurrentTarget.IsDead)
        {
            _stateMachine.SwitchState<IdleState>();
            return true;
        }

        return false;
    }

    private bool TrySwitchIfOutOfRange()
    {
        var distanceToTarget = _movement.DistanceToTarget;

        //if (distanceToTarget <= _stats.RangedAttackDistanceMax &&
        //    IsTargetInViewAngle())
        //{
        //    _lastTimeTargetSeen = Time.time;
        //}

        //float timeSinceLastSeenTarget = Time.time - _lastTimeTargetSeen;

        //if (timeSinceLastSeenTarget > _stats.MemoryOfPlayerDuration)
        //{
        //    _stateMachine.SwitchState<IdleState>();
        //    return true;
        //}

        if (distanceToTarget > _stats.RangedAttackDistanceMax)
        {
            _stateMachine.SwitchState<ChaseState>();
            return true;
        }

        return false;
    }
}