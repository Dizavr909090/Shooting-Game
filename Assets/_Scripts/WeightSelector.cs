using System.Collections.Generic;

public static class WeightSelector
{
    public static T GetRandom<T>(List<T> items) where T : IWeightable
    {
        if (items == null) 
            return default;

        if (items.Count == 1) 
            return items[0];

        float totalWeight = 0f;

        foreach (var weightableItem in items) 
            totalWeight += weightableItem.CurrentWeight;


        if (totalWeight == 0f) 
            return items[0];


        float randomPoint = UnityEngine.Random.Range(0f, totalWeight);

        foreach (var weightableItem in items)
        {
            if (randomPoint < weightableItem.CurrentWeight)
            {
                return weightableItem;
            }
            else
            {
                randomPoint -= weightableItem.CurrentWeight;
            }
        }

        return items[items.Count - 1];
    }
}
