using Core.Configs;
using UnityEngine;
using Sirenix.OdinInspector;

public class FishConfig : ScriptableObject, IValidatableConfig
{
    [Title("Основное")]
    public string Id = "goldfish_basic";

    [Title("Движение")]
    [MinMaxSlider(0.5f, 10f, true)] public Vector2 SpeedRange = new(1.5f, 3f);
    [Tooltip("Скорость сглаживания поворота визуала. Чем выше, тем быстрее рыбка 'клюет носом'.")]
    public float SteerSpeed = 3f;
    [Tooltip("Максимальный угол наклона (в градусах) при движении вверх или вниз.")]
    public float MaxTiltAngle = 30f;

    [Title("Визуал")]
    public Sprite Sprite;
    public string Type = "goldfish";
    public int Price = 100;
    [MinMaxSlider(0.5f, 3f, true)]
    public Vector2 SizeModifier = new(0.9f, 1.1f);

    [Title("Жизнь")]
    public float LifetimeSeconds = 45f;
    [Tooltip("Скорость роста голода (% в секунду).")]
    public float HungerGrowthPercentPerSecond = 20f;

    [Title("Размножение")]
    public float BreedChancePercent = 10f;
    public float BreedCooldownSeconds = 12f;

    [Title("Доход")]
    public int IncomeCoins = 3;
    public float IncomeCooldownSeconds = 5f;

    private void OnValidate()
    {
        ValidateData();
    }

    public void ValidateData()
    {
        SpeedRange.x = Mathf.Max(0.1f, SpeedRange.x);
        SpeedRange.y = Mathf.Max(SpeedRange.x, SpeedRange.y);

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