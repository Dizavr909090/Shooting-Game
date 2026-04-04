using UnityEngine;

[CreateAssetMenu(fileName = "FormationAttackAbilitySO", menuName = "Scriptable Objects/FormationAttackAbilitySO")]
public class FormationAttackAbilitySO : BaseAbilitySO
{
    [field: SerializeField] public PlateFormationSO Formation;
    [field: SerializeField] public ShootAbilitySO ShootSettings;

    public override BaseAbility CreateAbilityLogic(IAbilityUser abilityUser)
    {
        return new FormationAttackAbility(this, abilityUser);
    }
}
