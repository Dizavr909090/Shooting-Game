using System.Collections.Generic;
using UnityEngine;

public class AbilityController : MonoBehaviour, IAbilityUser, IModuleRegistry
{
    private List<IActorModule> _modules = new List<IActorModule>();

    public void ExecuteCommand(ICommand command)
    {
        foreach (var module in _modules)
        {
            module.HandleCommand(command);
        }
    }

    public void RegistryModule(IActorModule module)
    {
        _modules.Add(module);
    }

    public void UnRegistryModule(IActorModule module)
    {
        _modules.Remove(module);
    }

    public bool AllModulesReady()
    {
        foreach (var module in _modules)
        {
            if (!module.IsAtTarget) return false;
        }
        return true;
    }
}
