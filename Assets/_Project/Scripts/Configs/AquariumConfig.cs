using Core.Configs;
using UnityEngine;

public class AquariumConfig : ScriptableObject, IValidatableConfig
{
    public int StartCoins = 300;
    public int StartFishCount = 2;
    public int MaxFishCount = 10;
    public float DefaultEntitiesScale = 1f;
    public float FishesDefaultScale = 1f;
    public float CoinsDefaultScale = 1f;
    public float FoodDefaultScale = 1f;

    private void OnValidate()
    {
        ValidateData();
    }

    public void ValidateData()
    {
        StartCoins = Mathf.Max(0, StartCoins);
        StartFishCount = Mathf.Max(0, StartFishCount);
        MaxFishCount = Mathf.Max(1, MaxFishCount);
        DefaultEntitiesScale = Mathf.Clamp(DefaultEntitiesScale, 0.1f, 10f);
        FishesDefaultScale = Mathf.Clamp(FishesDefaultScale, 0.1f, 10f);
        CoinsDefaultScale = Mathf.Clamp(CoinsDefaultScale, 0.1f, 10f);
        FoodDefaultScale = Mathf.Clamp(FoodDefaultScale, 0.1f, 10f);

        if (StartFishCount > MaxFishCount)
            StartFishCount = MaxFishCount;
    }
}