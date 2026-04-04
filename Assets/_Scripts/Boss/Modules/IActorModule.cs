public interface IActorModule
{
    int ID { get; }
    bool IsAtTarget { get; } // Новое свойство
    void HandleCommand(ICommand command);
}
