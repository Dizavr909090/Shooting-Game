using UnityEngine;

[System.Serializable]
public abstract class BaseAbility : IWeightable
{
    protected IAbilityUser _abilityUser;

    [SerializeField] private BaseAbilitySO _baseData;
    [SerializeField] private float _currentWeight;
    

    public float CurrentWeight => _currentWeight;
    public string Name => _baseData != null ? _baseData.AbilityName : "Unknown Ability";
    public BaseAbilitySO BaseData => _baseData;

    public BaseAbility(BaseAbilitySO data, IAbilityUser abilityUser)
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

public abstract class BaseAbility<T> : BaseAbility where T : BaseAbilitySO
{
    protected T Data => (T)BaseData;

    protected BaseAbility(T data, IAbilityUser abilityUser) : base(data, abilityUser) { }

}