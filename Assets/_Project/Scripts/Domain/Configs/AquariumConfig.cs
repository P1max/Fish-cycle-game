using Core.Configs;
using Sirenix.OdinInspector;
using UnityEngine;

public class AquariumConfig : ScriptableObject, IValidatableConfig
{
    [TitleGroup("Стартовые значения", Alignment = TitleAlignments.Centered)]
    [BoxGroup("Стартовые значения/Данные")] public int StartCoins = 300;
    [BoxGroup("Стартовые значения/Данные")] public int StartFishCount = 2;
    [BoxGroup("Стартовые значения/Данные")] public int MaxFishCount = 10;

    [TitleGroup("Масштабирование (Scale)", Alignment = TitleAlignments.Centered)]
    [BoxGroup("Масштабирование (Scale)/Сущности")] public float DefaultEntitiesScale = 1f;
    [BoxGroup("Масштабирование (Scale)/Сущности")] public float FishesDefaultScale = 1f;
    [BoxGroup("Масштабирование (Scale)/Сущности")] public float CoinsDefaultScale = 1f;
    [BoxGroup("Масштабирование (Scale)/Сущности")] public float FoodDefaultScale = 1f;

    private void OnValidate() => ValidateData();

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