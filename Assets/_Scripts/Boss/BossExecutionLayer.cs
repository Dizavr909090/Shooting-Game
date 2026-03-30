using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossExecutionLayer : MonoBehaviour
{
    [SerializeField] private List<BossAbilitySO> _allAbilityConfigs;
    [SerializeField] private List<BaseAbility> _allAbilities;


    private void Start()
    {
        foreach (var abilityConfig in _allAbilityConfigs)
        {
            var ability = abilityConfig.CreateAbilityLogic();
            _allAbilities.Add(ability);
        }

        RunProbabilityTest(1000);
        StartCoroutine(AbilityTicker());
    }

    private IEnumerator AbilityTicker()
    {
        while (true)
        {
            yield return new WaitForSeconds(2);
            UseAbility();
        }
        
    }

    public void UseAbility()
    {
        var selectedAbility = WeightSelector.GetRandom(_allAbilities);

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

    private void RunProbabilityTest(int iterations)
    {
        Dictionary<string, int> stats = new Dictionary<string, int>();

        // Подготавливаем словарь
        foreach (var a in _allAbilities) stats[a.Name] = 0;

        // Крутим цикл
        for (int i = 0; i < iterations; i++)
        {
            var picked = WeightSelector.GetRandom(_allAbilities);
            stats[picked.Name]++;
            UpdateAllWeights(picked);
        }

        // Выводим результат
        string report = $"<b>Результаты теста ({iterations} итераций):</b>\n";
        foreach (var pair in stats)
        {
            float percent = (pair.Value / (float)iterations) * 100f;
            report += $"{pair.Key}: {pair.Value} раз ({percent:F1}%)\n";
        }
        Debug.Log(report);
    }
}
