using UnityEngine;

[CreateAssetMenu(fileName = "ShootAction_SO", menuName = "Scriptable Objects/ShootAction_SO")]
public class ShootAction_SO : BaseAction_SO
{
    [field: SerializeField] public GunModifier Modifier;

    public override Base_Action CreateAction(IAbilityUser user)
    {
        return new ShootAction(this, user);
    }
}
