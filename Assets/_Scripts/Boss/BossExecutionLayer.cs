using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossExecutionLayer : MonoBehaviour
{
    [SerializeField] private List<BaseAbilitySO> _allAbilityConfigs;
    [SerializeField] private List<BaseAbility> _allAbilities = new List<BaseAbility>();
    [SerializeField] private AbilityController _bossController;

    private void Start()
    {
        foreach (var abilityConfig in _allAbilityConfigs)
        {
            var ability = abilityConfig.CreateAbilityLogic(_bossController);
            _allAbilities.Add(ability);
        }
        StartCoroutine(AttackRoutine());
    }

    public IEnumerator AttackRoutine()
    {
        while (true)
        {
            UseAbility(out BaseAbility selectedAbility);

            yield return new WaitForSeconds(selectedAbility.BaseData.ActiveDuration);

            selectedAbility.Stop();

            yield return new WaitForSeconds(selectedAbility.BaseData.Cooldown);
        }
    }

    public void UseAbility(out BaseAbility selectedAbility)
    {
        selectedAbility = WeightSelector.GetRandom(_allAbilities);

        selectedAbility.Execute();

        Debug.Log($"<color=cyan>СЛЕДУЮЩИЙ ХОД:</color> <b>{selectedAbility.Name}</b> | Вес: {selectedAbility.CurrentWeight:F1}");

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
