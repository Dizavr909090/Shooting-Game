public class BaseAbility : IWeightable
{
    protected BossAbilitySO _data;

    private float _currentWeight;

    public float CurrentWeight => _currentWeight;

    public void Initialize()
    {
        _currentWeight = _data.BaseWeight;
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
}
