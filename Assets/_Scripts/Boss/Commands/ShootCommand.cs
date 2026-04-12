public class ShootCommand : ICommand
{
    public GunModifier GunModifier { get; set; }
    public int TargetID { get; set; }
    public bool IsFiring { get; set; }

    public ShootCommand(GunModifier gunmodifier, int id, bool isFiring)
    {
        GunModifier = gunmodifier;
        TargetID = id;
        IsFiring = isFiring;
    }
}
