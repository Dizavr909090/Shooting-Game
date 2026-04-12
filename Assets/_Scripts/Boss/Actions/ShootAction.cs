using Cysharp.Threading.Tasks;

public class ShootAction : Base_Action<ShootAction_SO>
{
    public ShootAction(ShootAction_SO data, IAbilityUser abilityUser) : base(data, abilityUser){}

    public override async UniTask Execute()
    {
        var command = new ShootCommand(Data.Modifier, -1, true);
        _abilityUser.ExecuteCommand(command);

        await UniTask.Yield();
    }

    public override void Cleanup()
    {
        base.Cleanup();

        _abilityUser.ExecuteCommand(new ShootCommand(GunModifier.Default, -1, false));
    }
}
