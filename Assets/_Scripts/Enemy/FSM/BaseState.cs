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
}
