using UnityEngine;

public class ShootAbility : BaseAbility<ShootAbilitySO>
{
    public ShootAbility(ShootAbilitySO data, IAbilityUser abilityUser) : base(data, abilityUser)
    {
    }

    public override void Execute()
    {
        var command = new ShootCommand(-1, true);
        _abilityUser.ExecuteCommand(command);
    }
}
