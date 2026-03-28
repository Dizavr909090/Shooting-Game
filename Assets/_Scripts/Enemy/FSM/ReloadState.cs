public class ReloadState : BaseState
{
    private EnemyMovement _movement;
    private IShootable _shootable;

    public ReloadState(StateMachine stateMachine, EnemyMovement movement, IShootable shootable) : base(stateMachine)
    {
        _movement = movement;
        _shootable = shootable; 
    }

    public override void OnEnter() 
    {
        _movement.StopMoving();
        _shootable.Reload();
    }

    public override void Update() 
    {
        if (!_stateMachine.AmmoProvider.IsReloading)
        {
            _stateMachine.SwitchState<IdleState>();
        }
    }

    public override void OnExit() 
    {
        _shootable.CancelReload();
    }
}