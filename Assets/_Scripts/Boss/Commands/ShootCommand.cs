public class ShootCommand : ICommand
{
    public int TargetID { get; set; }
    public bool IsFiring { get; set; }

    public ShootCommand(int id, bool isFiring)
    {
        TargetID = id;
        IsFiring = isFiring;
    }
}
