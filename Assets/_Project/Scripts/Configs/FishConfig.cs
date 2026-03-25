using System;
using Core.Configs;
using UnityEngine;
using Sirenix.OdinInspector;

[Serializable]
public class FishConfig : IValidatableConfig
{
    [BoxGroup("Основное", centerLabel: true)]
    public string Id = "goldfish_basic";

    [BoxGroup("Основное")]
    public string Type = "goldfish";

    [BoxGroup("Основное")]
    public int Price = 100;

    [BoxGroup("Основное"), PreviewField(50, ObjectFieldAlignment.Right)]
    public Sprite Sprite;

    [FoldoutGroup("Движение и Физика")]
    [MinMaxSlider(0.5f, 5f, true)]
    public Vector2 NormalSpeedRange = new(1.5f, 2f);

    [FoldoutGroup("Движение и Физика")]
    public float MaxHungrySpeed = 3f;

    [FoldoutGroup("Движение и Физика")]
    public float SteerSpeed = 7f;

    [FoldoutGroup("Движение и Физика")]
    public float MaxTiltAngle = 20f;

    [FoldoutGroup("Движение и Физика")]
    [MinMaxSlider(0.5f, 3f, true)]
    public Vector2 SizeModifier = new(0.9f, 1.1f);

    [FoldoutGroup("Жизненный цикл")]
    public float LifetimeSeconds = 45f;

    [FoldoutGroup("Жизненный цикл")]
    public float HungerGrowthPercentPerSecond = 20f;

    [FoldoutGroup("Размножение")]
    public float BreedChancePercent = 10f;

    [FoldoutGroup("Размножение")]
    public float BreedCooldownSeconds = 12f;

    [FoldoutGroup("Экономика")]
    public int IncomeCoins = 3;

    [FoldoutGroup("Экономика")]
    public float IncomeCooldownSeconds = 5f;

    public FishConfig Clone() => (FishConfig)MemberwiseClone();

    public void ValidateData()
    {
        NormalSpeedRange.x = Mathf.Max(0.1f, NormalSpeedRange.x);
        NormalSpeedRange.y = Mathf.Max(NormalSpeedRange.x, NormalSpeedRange.y);
        MaxHungrySpeed = Mathf.Max(NormalSpeedRange.y, MaxHungrySpeed);

        SteerSpeed = Mathf.Max(0f, SteerSpeed);
        MaxTiltAngle = Mathf.Clamp(MaxTiltAngle, 0f, 60f);

        Price = Mathf.Max(0, Price);

        SizeModifier.x = Mathf.Max(0.1f, SizeModifier.x);
        SizeModifier.y = Mathf.Max(SizeModifier.x, SizeModifier.y);

        LifetimeSeconds = Mathf.Max(1f, LifetimeSeconds);
        HungerGrowthPercentPerSecond = Mathf.Max(0f, HungerGrowthPercentPerSecond);

        BreedChancePercent = Mathf.Clamp(BreedChancePercent, 0f, 100f);
        BreedCooldownSeconds = Mathf.Max(0f, BreedCooldownSeconds);

        IncomeCoins = Mathf.Max(0, IncomeCoins);
        IncomeCooldownSeconds = Mathf.Max(0.1f, IncomeCooldownSeconds);
    }
}