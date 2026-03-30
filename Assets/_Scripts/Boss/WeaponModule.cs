using UnityEngine;
public class WeaponModule : IBossModule
{
    public void HandleCommand(IBossCommand command)
    {
        if (command is AttackCommand)
        {
            Debug.Log("FIRE");
        }
    }
}
