using System.Collections.Generic;
using UnityEngine;

public class BossExecutionLayer : MonoBehaviour
{
    private List<BaseAbility> _allAbilities;

    public void UseAbility()
    {
        var selectedAbility = WeightSelector.GetRandom(_allAbilities);

        UpdateAllWeights(selectedAbility);
    }

    private void UpdateAllWeights(BaseAbility choseAbility)
    {
        choseAbility.PerformPenaltyMultiplierForWeight();

        foreach (BaseAbility ability in _allAbilities)
        {
            if (ability == choseAbility) continue;

            ability.IncreaseValueOfWeight();
        }
    }
}
