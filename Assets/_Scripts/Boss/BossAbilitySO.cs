using UnityEngine;

[CreateAssetMenu(fileName = "BossAbilitySO", menuName = "Scriptable Objects/BossAbilitySO")]
public abstract class BossAbilitySO : ScriptableObject
{
    [Header("Identification")]
    public string AbilityName;

    [Header("AI Weights")]
    public float BaseWeight;
    public float PenaltyMultiplier;
    public float RecoveryValue;

    [Header("Usage Conditions")]
    public float Cooldown;
    public float MinDistance;
    public float MaxDistance;

    public abstract BaseAbility CreateAbilityLogic(BossStateMachine stateMachine);
}
