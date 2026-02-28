public abstract class BaseState : IState
{
    protected readonly StateMachine _stateMachine;

    protected BaseState(StateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public abstract void OnEnter();

    public abstract void OnExit();

    public abstract void Update();

    protected bool HandleTargetLost()
    {
        if (_stateMachine.CurrentTarget == null || _stateMachine.CurrentTarget.IsDead)
        {
            _stateMachine.SwitchState<IdleState>();
            return true;
        }

        return false;
    }
}
