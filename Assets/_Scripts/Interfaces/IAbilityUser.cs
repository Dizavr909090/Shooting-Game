using System.Collections;
using UnityEngine;

public interface IAbilityUser
{
    void ExecuteCommand(ICommand command);
    bool AllModulesReady();
    Coroutine StartCoroutine(IEnumerator routine);
}
