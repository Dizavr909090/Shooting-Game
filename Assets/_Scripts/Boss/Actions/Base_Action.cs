using Cysharp.Threading.Tasks;
using System.Collections;
using UnityEngine;

public abstract class Base_Action : MonoBehaviour
{
    protected IAbilityUser _abilityUser;

    protected Base_Action(IAbilityUser abilityUser)
    {
        _abilityUser = abilityUser;
    }

    public abstract UniTask Execute();
}
