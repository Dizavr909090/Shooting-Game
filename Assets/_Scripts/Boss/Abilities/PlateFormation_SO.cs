using UnityEngine;

[CreateAssetMenu(fileName = "PlateFormation_SO", menuName = "Scriptable Objects/PlateFormation_SO")]
public class PlateFormation_SO : BaseAction_SO
{
    [field: SerializeField] public float[] PlateAngles;
    [field: SerializeField] public Vector3[] WeaponOffsets;

    public override Base_Action CreateAction(IAbilityUser user)
    {
        return new MovePlateAction(this, user);
    }
}
