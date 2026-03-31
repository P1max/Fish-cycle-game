using System;
using System.Collections.Generic;
using Core.Configs;
using Sirenix.OdinInspector;
using UnityEngine;

public class UpgradesConfig : ScriptableObject, IValidatableConfig
{
    [BoxGroup("Формулы: Фон")]
    [Tooltip("Начальный масштаб фона (например, 1.5 - сильно приближен)")]
    public float InitialBackgroundScale = 1.5f;

    [Title("Уровни прокачки аквариума")]
    [TableList(ShowIndexLabels = true, AlwaysExpanded = true)]
    public List<LevelData> LevelsUpgrades = new();

    public int TotalLevels => LevelsUpgrades.Count;

    private void OnValidate() => ValidateData();

    public void ValidateData()
    {
    }

    [Serializable]
    public class LevelData
    {
        public int Level;
        public int UpgradeCost;
        public int NewMaxFishesCount;
        public float NewAquariumScale;
        public int FoodCountIncrementToDefault;
        public int HungerRestorePerUseIncrementToDefault;
        public float NewFeederCooldown;
    }
}