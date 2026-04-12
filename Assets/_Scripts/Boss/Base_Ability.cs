using UnityEngine;

[System.Serializable]
public abstract class Base_Ability : IWeightable
{
    protected IAbilityUser _abilityUser;

    [SerializeField] private BaseAbility_SO _baseData;
    [SerializeField] private float _currentWeight;

    public float CurrentWeight => _currentWeight;
    public string Name => _baseData != null ? _baseData.AbilityName : "Unknown Ability";
    public BaseAbility_SO BaseData => _baseData;

    public Base_Ability(BaseAbility_SO data, IAbilityUser abilityUser)
    {
        _baseData = data;
        _currentWeight = data.BaseWeight;
        _abilityUser = abilityUser;
    }

    public void IncreaseValueOfWeight()
    {
        _currentWeight += _baseData.RecoveryValue;
    }

    public void PerformPenaltyMultiplierForWeight()
    {
        var penaliedWeight = _baseData.BaseWeight * _baseData.PenaltyMultiplier;

        if (_currentWeight > penaliedWeight)
        {
            _currentWeight = penaliedWeight;
        }
        else
        {
            _currentWeight *= _baseData.PenaltyMultiplier;
        }
    }

    public abstract void Execute();
    public virtual void Stop() { }
}

public abstract class Base_Ability<T> : Base_Ability where T : BaseAbility_SO
{
    protected T Data => (T)BaseData;

    protected Base_Ability(T data, IAbilityUser abilityUser) : base(data, abilityUser) { }

}