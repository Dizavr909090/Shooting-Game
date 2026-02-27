using Unity.VisualScripting;
using UnityEngine;

public class RangedAttackState : BaseState
{
    private IShootable _shootable;
    private EnemyStats _stats;
    private EnemyMovement _movement;

    public RangedAttackState(
        StateMachine stateMachine, 
        IShootable shootable, 
        EnemyStats stats,
        EnemyMovement movement) : base(stateMachine)
    {
        _shootable = shootable;
        _stats = stats;
        _movement = movement;
    }

    public override void OnEnter()
    {
        _movement.StopMoving();
    }

    public override void Update()
    {
        if (HandleTargetLost()) return;
        if (TrySwitchIfOutOfRange()) return;


        if (_shootable.CanShoot) _shootable.Shoot();
    }

    public override void OnExit()
    {

    }

    private bool TrySwitchIfOutOfRange()
    {
        var distanceToTarget = _movement.DistanceToTarget;

        if (distanceToTarget > _stats.RangedAttackDistanceMax)
        {
            _stateMachine.SwitchState<ChaseState>();
            return true;
        }

        return false;
    }

    private bool IsTargetInViewAngle()
    {
        if (HandleTargetLost()) return false;

        Vector3 directionToTarget = (_stateMachine.CurrentTarget.Transform.position -
            _movement.transform.position).normalized;

        float angle = Vector3.Angle(_movement.transform.forward, directionToTarget);

        float halfViewAngle = _stats.ViewAngle * 0.5f;

        return angle <= halfViewAngle;
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
}