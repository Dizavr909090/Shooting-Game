public class IdleState : BaseState
{
    private EnemyMovement _movement;

    public IdleState(
        StateMachine stateMachine, 
        EnemyMovement movement) : base(stateMachine)
    {
        _movement = movement;
    }

    public override void OnEnter()
    {
        _movement.StopMoving();
    }

    public override void Update()
    {
        if (_stateMachine.CurrentTarget != null)
        {
            _stateMachine.SwitchState<ChaseState>();
        }
    }

    public override void OnExit()
    {

    }
}
