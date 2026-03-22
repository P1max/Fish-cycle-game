using Core.Configs;
using Sirenix.OdinInspector;
using UnityEngine;

public class BoidsConfig : ScriptableObject, IValidatableConfig
{
    [Title("Поведение у границ (Viewport)")]
    public float EdgeMargin = 0.2f;
    public float BoundsWeight = 23f;

    [Title("Boids (Стая)")]
    public float NeighborRadius = 3f;
    public float SeparationRadius = 3f;
    public float AlignmentWeight = 5f;
    public float CohesionWeight = 2f;
    public float SeparationWeight = 12.5f;
    
    [Title("Еда")]
    public float FoodWeight = 50f;
    public float FoodSearchRadius = 5f;

    [Title("Своё мнение")]
    public float WanderWeight = 13f;

    public void ValidateData()
    {
        EdgeMargin = Mathf.Clamp(EdgeMargin, 0f, 0.49f);
        BoundsWeight = Mathf.Max(0f, BoundsWeight);
        NeighborRadius = Mathf.Max(0.1f, NeighborRadius);
        SeparationRadius = Mathf.Max(0.1f, SeparationRadius);
        AlignmentWeight = Mathf.Max(0f, AlignmentWeight);
        CohesionWeight = Mathf.Max(0f, CohesionWeight);
        SeparationWeight = Mathf.Max(0f, SeparationWeight);
        WanderWeight = Mathf.Max(0f, WanderWeight);
        FoodWeight = Mathf.Max(0f, FoodWeight);
        FoodSearchRadius = Mathf.Max(0.1f, FoodSearchRadius);
    }
}