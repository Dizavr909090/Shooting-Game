using UnityEngine;

[System.Serializable]
public class BaseAbility : IWeightable
{
    [SerializeField] protected BossAbilitySO _data;
    [SerializeField] private float _currentWeight;
    private IAbilityUser _abilityUser;

    public float CurrentWeight => _currentWeight;
    public string Name => _data != null ? _data.AbilityName : "Unknown Ability";

    public BaseAbility(BossAbilitySO data, IAbilityUser abilityUser)
    {
        _data = data;
        _currentWeight = data.BaseWeight;
        _abilityUser = abilityUser;
    }

    public void IncreaseValueOfWeight()
    {
        _currentWeight += _data.RecoveryValue;
    }

    public void PerformPenaltyMultiplierForWeight()
    {
        var penaliedWeight = _data.BaseWeight * _data.PenaltyMultiplier;

        if (_currentWeight > penaliedWeight)
        {
            _currentWeight = penaliedWeight;
        }
        else
        {
            _currentWeight *= _data.PenaltyMultiplier;
        }
    }

    public void Execute()
    {
        _abilityUser.ExecuteCommand(new AttackCommand());
    }
}
