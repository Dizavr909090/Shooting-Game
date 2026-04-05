using UnityEngine;

[CreateAssetMenu(fileName = "BaseAction_SO", menuName = "Scriptable Objects/BaseAction_SO")]
public abstract class BaseAction_SO : ScriptableObject
{
    public abstract Base_Action CreateAction(IAbilityUser user);
}
