public interface IActorModule
{
    int ModuleID { get; }
    bool IsAtTarget { get; }
    void HandleCommand(ICommand command);
}
