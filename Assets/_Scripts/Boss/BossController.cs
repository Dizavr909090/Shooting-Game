using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour, IAbilityUser
{
    private List<IBossModule> Modules;

    public void ExecuteCommand(IBossCommand command)
    {
        foreach (var module in Modules)
        {
            module.HandleCommand(command);
        }
    }
}
