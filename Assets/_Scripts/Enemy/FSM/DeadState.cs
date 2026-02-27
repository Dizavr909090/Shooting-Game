public class DeadState : BaseState
{
    private EnemyMovement _movement;

    public DeadState(StateMachine stateMachine, EnemyMovement movement) : base(stateMachine)
    {
        _movement = movement;
    }

    public override void OnEnter() 
    {
        _movement.StopMoving();
        _movement.SetKinematic(true);
    }

    public override void Update() { }

    public override void OnExit() 
    {
        _movement.SetKinematic(false);
    }
}