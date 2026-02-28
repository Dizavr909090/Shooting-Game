public interface ITargetProvider
{
    ITargetable Target { get; }
    bool IsVisible { get; }
}
