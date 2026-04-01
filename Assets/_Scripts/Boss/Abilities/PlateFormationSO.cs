using UnityEngine;

[CreateAssetMenu(fileName = "PlateFormationSO", menuName = "Scriptable Objects/PlateFormationSO")]
public class PlateFormationSO : BaseAbilitySO
{
    [field: SerializeField] public float[] PlateAngles;
    [field: SerializeField] public Vector3[] WeaponOffsets;

    public override BaseAbility CreateAbilityLogic(IAbilityUser abilityUser)
    {
        return new FormationAbility(this, abilityUser);
    }
}
