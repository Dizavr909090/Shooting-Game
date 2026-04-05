using UnityEngine;

public abstract class BaseAbility_SO : ScriptableObject
{
    [Header("Identification")]
    [field: SerializeField]public string AbilityName { get; private set; }

    [Header("AI Weights")]
    [field: SerializeField] public float BaseWeight { get; private set; }
    [field: SerializeField] public float PenaltyMultiplier { get; private set; }
    [field: SerializeField] public float RecoveryValue { get; private set; }

    [Header("Usage Conditions")]
    [field: SerializeField] public float ActiveDuration { get; private set; }
    [field: SerializeField] public float Cooldown { get; private set; }
    [field: SerializeField] public float MinDistance { get; private set; }
    [field: SerializeField] public float MaxDistance { get; private set; }

    public abstract Base_Ability CreateAbilityLogic(IAbilityUser abilityUser);
}
