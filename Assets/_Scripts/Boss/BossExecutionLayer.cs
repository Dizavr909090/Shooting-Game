using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossExecutionLayer : MonoBehaviour
{
    [SerializeField] private List<BaseAbility_SO> _allAbilityConfigs;
    [SerializeField] private List<Base_Ability> _allAbilities = new List<Base_Ability>();
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
            UseAbility(out Base_Ability selectedAbility);

            yield return new WaitForSeconds(selectedAbility.BaseData.ActiveDuration);

            selectedAbility.Stop();

            yield return new WaitForSeconds(selectedAbility.BaseData.Cooldown);
        }
    }

    public void UseAbility(out Base_Ability selectedAbility)
    {
        selectedAbility = WeightSelector.GetRandom(_allAbilities);

        selectedAbility.Execute();

        Debug.Log($"<color=cyan>СЛЕДУЮЩИЙ ХОД:</color> <b>{selectedAbility.Name}</b> | Вес: {selectedAbility.CurrentWeight:F1}");

        UpdateAllWeights(selectedAbility);
    }

    private void UpdateAllWeights(Base_Ability choseAbility)
    {
        choseAbility.PerformPenaltyMultiplierForWeight();

        foreach (Base_Ability ability in _allAbilities)
        {
            if (ability == choseAbility) continue;

            ability.IncreaseValueOfWeight();
        }
    }
}
