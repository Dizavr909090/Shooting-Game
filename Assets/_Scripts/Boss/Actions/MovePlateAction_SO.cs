using UnityEngine;

[CreateAssetMenu(fileName = "MovePlateAction_SO", menuName = "Scriptable Objects/MovePlateAction_SO")]
public class MovePlateAction_SO : BaseAction_SO
{
    [field: SerializeField] public PlateFormation_SO Formation;

    public override Base_Action CreateAction(IAbilityUser user)
        => new MovePlateAction(Formation, user);
}
