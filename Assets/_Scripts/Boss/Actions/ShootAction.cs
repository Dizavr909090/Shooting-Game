using Cysharp.Threading.Tasks;

public class ShootAction : Base_Action
{
    public ShootAction(ShootAction_SO data, IAbilityUser abilityUser) : base(abilityUser){}

    public override async UniTask Execute()
    {
        var command = new ShootCommand(-1, true);
        _abilityUser.ExecuteCommand(command);

        await UniTask.Yield();
    }
}
