using Core.Configs;
using Sirenix.OdinInspector;
using UnityEngine;

public class CommonFishConfig : ScriptableObject, IValidatableConfig
{
    [BoxGroup("Поведение у границ (Viewport)")]
    public float MarginXSides = 0.1f;
    [BoxGroup("Поведение у границ (Viewport)")]
    public float MarginTop = 0.2f;
    [BoxGroup("Поведение у границ (Viewport)")]
    public float MarginBottom = 0.15f;
    [BoxGroup("Поведение у границ (Viewport)")]
    public float BoundsWeight = 23f;

    [BoxGroup("Boids (Стая)")]
    public float NeighborRadius = 3f;
    [BoxGroup("Boids (Стая)")]
    public float SeparationRadius = 3f;
    [BoxGroup("Boids (Стая)")]
    public float AlignmentWeight = 5f;
    [BoxGroup("Boids (Стая)")]
    public float CohesionWeight = 2f;
    [BoxGroup("Boids (Стая)")]
    public float SeparationWeight = 12.5f;

    [BoxGroup("Потомство")]
    public float PartnerSearchRadius = 1f;

    [BoxGroup("Препятствия")]
    public float ObstacleAvoidanceWeight = 30f;

    [BoxGroup("Еда")]
    public float FoodWeight = 50f;
    [BoxGroup("Еда")]
    public float FoodSearchRadius = 5f;
    [BoxGroup("Еда")]
    [MinMaxSlider(0.5f, 5f, true)]
    public Vector2 EatTimer = new(2f, 3f);

    [BoxGroup("Своё мнение")]
    public float WanderWeight = 13f;

    [BoxGroup("UI Индикаторы")]
    [Tooltip("За сколько секунд до смерти появляется таймер")]
    public float DeathTimerThreshold = 10f;

    [BoxGroup("UI Индикаторы")]
    [Tooltip("При каком проценте голода (0-100) появляется иконка еды")]
    [PropertyRange(0f, 100f)]
    public float HungerIndicatorThreshold = 80f;

    private void OnValidate()
    {
        ValidateData();
    }

    public void ValidateData()
    {
        MarginXSides = Mathf.Clamp(MarginXSides, 0f, 0.49f);
        MarginTop = Mathf.Clamp(MarginTop, 0f, 0.49f);
        MarginBottom = Mathf.Clamp(MarginBottom, 0f, 0.49f);
        BoundsWeight = Mathf.Max(0f, BoundsWeight);
        NeighborRadius = Mathf.Max(0.1f, NeighborRadius);
        SeparationRadius = Mathf.Clamp(SeparationRadius, 0.1f, NeighborRadius);
        PartnerSearchRadius = Mathf.Clamp(PartnerSearchRadius, 0.1f, NeighborRadius);
        AlignmentWeight = Mathf.Max(0f, AlignmentWeight);
        CohesionWeight = Mathf.Max(0f, CohesionWeight);
        SeparationWeight = Mathf.Max(0f, SeparationWeight);
        WanderWeight = Mathf.Max(0f, WanderWeight);
        FoodWeight = Mathf.Max(0f, FoodWeight);
        FoodSearchRadius = Mathf.Max(0.1f, FoodSearchRadius);

        DeathTimerThreshold = Mathf.Max(0.1f, DeathTimerThreshold);
        HungerIndicatorThreshold = Mathf.Clamp(HungerIndicatorThreshold, 0f, 100f);
    }
}