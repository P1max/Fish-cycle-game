using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New Fish Config", menuName = "Aquarium/Fish Config")]
public class FishConfig : ScriptableObject
{
    [Title("Движение")]
    [Tooltip("Диапазон случайной начальной скорости и жесткие границы скорости при движении.")]
    [MinMaxSlider(0.5f, 10f, true)] public Vector2 SpeedRange = new(1.5f, 3f);

    [Tooltip("Скорость сглаживания поворота визуала. Чем выше, тем быстрее рыбка 'клюет носом'.")]
    public float SteerSpeed = 3f;

    [Tooltip("Максимальный угол наклона (в градусах) при движении вверх или вниз.")]
    public float MaxTiltAngle = 30f;

    [Title("Поведение у границ (Viewport)")]
    [Tooltip("Расстояние от края экрана (0.1 = 10%), где рыбка начинает чувствовать границу аквариума.")]
    public float EdgeMargin = 0.1f;

    [Tooltip("Множитель скорости у краев. 0.5 замедлит рыбку вдвое, делая разворот более плавным.")]
    public float EdgeSlowdownFactor = 0.5f;

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

    [Tooltip("Сила случайного 'блуждания'. Добавляет хаоса в движение, чтобы рыбы не плыли по линейке.")]
    public float WanderWeight = 1f;
}