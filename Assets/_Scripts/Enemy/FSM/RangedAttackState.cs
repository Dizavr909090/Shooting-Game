using UnityEngine;

public class RangedAttackState : BaseState
{
    private Vector3 _lastVisiblePosition;
    private float _lostVisibilityTimer;
    private const float VisibilityTimeout = 0.5f;

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

        bool isVisible = _stateMachine.IsTargetVisible;

        if (!isVisible)
        {
            _lostVisibilityTimer += Time.deltaTime;

            if (_lostVisibilityTimer >= VisibilityTimeout)
            {
                _stateMachine.SwitchState<ChaseState>();
                return;
            }
        }
        else
        {
            _lostVisibilityTimer = 0;
        }

        if (TrySwitchIfOutOfRange()) return;

        if (_stateMachine.IsTargetVisible)
        {
            _lastVisiblePosition = _stateMachine.CurrentTarget.Transform.position;
        }

        Vector3 lookAtPos = _stateMachine.IsTargetVisible
            ? _stateMachine.CurrentTarget.Transform.position
            : _lastVisiblePosition;

        Vector3 targetPos = _stateMachine.CurrentTarget.Transform.position;
        _rotator.RotateTowards(lookAtPos);

        if (_rotator.IsFacingTarget(lookAtPos))
        {
            bool canSuppress = !isVisible && _lostVisibilityTimer < VisibilityTimeout;

            if ((isVisible || canSuppress) && _shootable.CanShoot)
            {
                _shootable.Shoot();
            }
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