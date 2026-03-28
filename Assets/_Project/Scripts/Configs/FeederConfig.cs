using Core.Configs;
using UnityEngine;
using Sirenix.OdinInspector;

public class FeederConfig : ScriptableObject, IValidatableConfig
{
    [Tooltip("Время восстановления кормушки в секундах.")]
    public float CooldownSeconds = 8f;

    [Tooltip("Общее количество сытости, которое дает одно нажатие на кормушку.")]
    public int TotalHungerRestorePerUse = 140;

    [Tooltip("Минимальное и максимальное количество частичек корма за один клик.")]
    [MinMaxSlider(1, 15, true)]
    public Vector2Int FoodPiecesCount = new(3, 6);

    private void OnValidate()
    {
        ValidateData();
    }

    public void ValidateData()
    {
        CooldownSeconds = Mathf.Max(0.1f, CooldownSeconds);
        TotalHungerRestorePerUse = Mathf.Max(1, TotalHungerRestorePerUse);
        
        if (FoodPiecesCount.x < 1) FoodPiecesCount.x = 1;
        if (FoodPiecesCount.y < FoodPiecesCount.x) FoodPiecesCount.y = FoodPiecesCount.x;
    }
}