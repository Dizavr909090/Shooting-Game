using UnityEngine;

public class RangedAttackState : BaseState
{
    private IShootable _shootable;
    private EnemyStats _stats;
    private EnemyMovement _movement;
    private EnemyRotator _rotator;

    public RangedAttackState(
        StateMachine stateMachine,
        EnemyMovement movement,
        EnemyStats stats,
        IShootable shootable,
        EnemyRotator rotator) : base(stateMachine)
    {
        _shootable = shootable;
        _stats = stats;
        _movement = movement;
        _rotator = rotator;
    }

    public override void OnEnter()
    {
        _movement.StopMoving();
    }

    public override void Update()
    {
        if (HandleTargetLost()) return;
        if (TrySwitchIfOutOfRange()) return;

        Vector3 targetPos = _stateMachine.CurrentTarget.Transform.position;
        _rotator.RotateTowards(targetPos);

        if (_rotator.IsFacingTarget(targetPos))
        {
            if (_shootable.CanShoot) _shootable.Shoot();
        }
    }

    public override void OnExit()
    {

    }

    private bool TrySwitchIfOutOfRange()
    {
        if (_stateMachine.DistanceToTarget > _stats.RangedAttackDistanceMax)
        {
            _stateMachine.SwitchState<ChaseState>();
            return true;
        }

        return false;
    }
}