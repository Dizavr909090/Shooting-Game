using UnityEngine;

[CreateAssetMenu(fileName = "ShootAbilitySO", menuName = "Scriptable Objects/ShootAbilitySO")]
public class ShootAbilitySO : BaseAbilitySO
{
    public override BaseAbility CreateAbilityLogic(IAbilityUser abilityUser)
    {
        return new ShootAbility(this, abilityUser);
    }
}
