using Core.Configs;
using Sirenix.OdinInspector;
using UnityEngine;

public class CommonFishConfig : ScriptableObject, IValidatableConfig
{
    [Title("Поведение у границ (Viewport)")]
    public float MarginXSides = 0.1f;
    public float MarginTop = 0.2f;
    public float MarginBottom = 0.15f;
    public float BoundsWeight = 23f;

    [Title("Boids (Стая)")]
    public float NeighborRadius = 3f;
    public float SeparationRadius = 3f;
    public float AlignmentWeight = 5f;
    public float CohesionWeight = 2f;
    public float SeparationWeight = 12.5f;
    
    [Title("Потомство")]
    public float PartnerSearchRadius = 1f;
    
    [Title("Препятствия")]
    public float ObstacleAvoidanceWeight = 30f;

    [Title("Еда")]
    public float FoodWeight = 50f;
    public float FoodSearchRadius = 5f;
    [MinMaxSlider(0.5f, 3f, true)]
    public Vector2 EatTimer = new(2f, 3f);

    [Title("Своё мнение")]
    public float WanderWeight = 13f;

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
    }
}