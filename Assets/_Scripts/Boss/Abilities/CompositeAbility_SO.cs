using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CompositeAbility_SO", menuName = "Scriptable Objects/CompositeAbility_SO")]
public class CompositeAbility_SO : BaseAbility_SO
{
    [SerializeField] private List<BaseAction_SO> _actionData;
    
    public override Base_Ability CreateAbilityLogic(IAbilityUser abilityUser)
    {
        var ability = new Composite_Ability(this, abilityUser);

        foreach (var data in _actionData)
        {
            ability.AddAction(data.CreateAction(abilityUser));
        }

        return ability;
    }
}
