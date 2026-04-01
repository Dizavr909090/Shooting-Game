using UnityEngine;

public class WeaponModule : BaseModule
{
    public override void HandleCommand(ICommand command)
    {
        if (command is AttackCommand attackCmd)
        {
            Debug.Log($"FIRE! Damage: {attackCmd.Damage}");
        }
    }
}
