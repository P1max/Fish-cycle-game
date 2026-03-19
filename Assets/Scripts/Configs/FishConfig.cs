using Core.Configs;
using UnityEngine;
using Sirenix.OdinInspector;

public class FishConfig : ScriptableObject, IValidatableConfig
{
    [Title("Основное")]
    [Tooltip("Уникальный идентификатор рыбки.")]
    public string Id = "goldfish_basic";
    
    [Title("Движение")]
    [Tooltip("Диапазон случайной начальной скорости и жесткие границы скорости при движении.")]
    [MinMaxSlider(0.5f, 10f, true)]
    public Vector2 SpeedRange = new(1.5f, 3f);

    [Tooltip("Скорость сглаживания поворота визуала. Чем выше, тем быстрее рыбка 'клюет носом'.")]
    public float SteerSpeed = 3f;

    [Tooltip("Максимальный угол наклона (в градусах) при движении вверх или вниз.")]
    public float MaxTiltAngle = 30f;

    [Title("Поведение у границ (Viewport)")]
    [Tooltip("Расстояние от края экрана (0.1 = 10%), где рыбка начинает чувствовать границу аквариума.")]
    public float EdgeMargin = 0.1f;

    [Tooltip("Сила, выталкивающая рыбку от границ обратно в центр аквариума.")]
    public float BoundsWeight = 5f;

    [Title("Boids (Стая)")]
    [Tooltip("Радиус 'обзора', в котором рыба замечает соседей для взаимодействия.")]
    public float NeighborRadius = 2f;
    [Tooltip("Дистанция личного пространства. Если сосед ближе, рыба начнет от него отплывать.")]
    public float SeparationRadius = 0.75f;

    [Tooltip("Насколько сильно рыбка хочет плыть в том же направлении, что и стая вокруг.")]
    public float AlignmentWeight = 1f;

    [Tooltip("Желание рыбки держаться ближе к геометрическому центру группы соседей.")]
    public float CohesionWeight = 1f;

    [Tooltip("Приоритет сохранения дистанции. Обычно это самый высокий вес, чтобы рыбы не слипались.")]
    public float SeparationWeight = 2f;

    [Title("Своё мнение")]
    [Tooltip("Сила случайного 'блуждания'. Добавляет хаоса в движение, чтобы рыбы не плыли по линейке.")]
    public float WanderWeight = 1f;

    [Title("Визуал")]
    [Tooltip("Спрайт рыбки")]
    public Sprite Sprite;

    [Tooltip("Тип рыбки (используется для подбора правильного спрайта).")]
    public string Type = "goldfish";

    [Tooltip("Цена покупки в конвейере.")]
    public int Price = 100;

    [Tooltip("Модификатор размера (мин и макс). При спавне базовый размер умножится на это значение.")]
    [MinMaxSlider(0.5f, 3f, true)]
    public Vector2 SizeModifier = new(0.9f, 1.1f);

    [Tooltip("Время жизни рыбки в секундах до естественной смерти.")]
    public float LifetimeSeconds = 45f;

    [Tooltip("Скорость роста голода (% в секунду).")]
    public float HungerGrowthPercentPerSecond = 20f;

    [Title("Размножение")]
    [Tooltip("Шанс на успешное размножение при встрече двух готовых рыб (в %).")]
    public float BreedChancePercent = 10f;

    [Tooltip("Время в секундах между попытками размножиться.")]
    public float BreedCooldownSeconds = 12f;

    [Title("Доход")]
    [Tooltip("Количество монет, которое рыбка производит за один цикл.")]
    public int IncomeCoins = 3;

    [Tooltip("Время в секундах между генерацией монет.")]
    public float IncomeCooldownSeconds = 5f;

    [Title("Еда")]
    [Tooltip("Вес притяжения к еде")]
    public float FoodWeight = 50f;

    [Tooltip("Радиус, в котором рыба замечает еду")]
    public float FoodSearchRadius = 5f;

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
        
        EdgeMargin = Mathf.Clamp(EdgeMargin, 0f, 0.49f);
        BoundsWeight = Mathf.Max(0f, BoundsWeight);

        NeighborRadius = Mathf.Max(0.1f, NeighborRadius);
        SeparationRadius = Mathf.Max(0.1f, SeparationRadius);

        AlignmentWeight = Mathf.Max(0f, AlignmentWeight);
        CohesionWeight = Mathf.Max(0f, CohesionWeight);
        SeparationWeight = Mathf.Max(0f, SeparationWeight);
        WanderWeight = Mathf.Max(0f, WanderWeight);

        Price = Mathf.Max(0, Price);
        
        SizeModifier.x = Mathf.Max(0.1f, SizeModifier.x);
        SizeModifier.y = Mathf.Max(SizeModifier.x, SizeModifier.y);

        LifetimeSeconds = Mathf.Max(1f, LifetimeSeconds);
        HungerGrowthPercentPerSecond = Mathf.Max(0f, HungerGrowthPercentPerSecond);

        BreedChancePercent = Mathf.Clamp(BreedChancePercent, 0f, 100f);
        BreedCooldownSeconds = Mathf.Max(0f, BreedCooldownSeconds);

        IncomeCoins = Mathf.Max(0, IncomeCoins);
        IncomeCooldownSeconds = Mathf.Max(0.1f, IncomeCooldownSeconds);

        FoodWeight = Mathf.Max(0f, FoodWeight);
        FoodSearchRadius = Mathf.Max(0.1f, FoodSearchRadius);
    }
}