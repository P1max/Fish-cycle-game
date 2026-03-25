using Core.Configs;
using UnityEngine;

public class AquariumConfig : ScriptableObject, IValidatableConfig
{
    public int StartCoins = 300;
    public int StartFishCount = 2;
    public int MaxFishCount = 10;

    private void OnValidate()
    {
        ValidateData();
    }

    public void ValidateData()
    {
        StartCoins = Mathf.Max(0, StartCoins);
        StartFishCount = Mathf.Max(0, StartFishCount);
        MaxFishCount = Mathf.Max(1, MaxFishCount);

        if (StartFishCount > MaxFishCount)
            StartFishCount = MaxFishCount;
    }
}