using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Core.Interfaces;
using UnityEngine;

namespace Core.Game.Upgrade
{
    public class UpgradeManager : IGameplayInit
    {
        private readonly UpgradesConfig _upgradesConfig;

        private Dictionary<int, UpgradesConfig.LevelData> _levelsMap;
        private int _currentLevel;

        public event Action<UpgradesConfig.LevelData> OnAquariumUpgrade;

        public UpgradesConfig.LevelData CurrentLevelData { get; private set; }
        public UpgradesConfig.LevelData NextLevelData { get; private set; }

        public int CurrentLevel => _currentLevel;
        public bool IsMaxLevel => NextLevelData == null;
        public int NextLevelCost => NextLevelData?.UpgradeCost ?? int.MaxValue;

        public UpgradeManager(UpgradesConfig upgradesConfig)
        {
            _upgradesConfig = upgradesConfig;
        }

        private void SetLevel(int level)
        {
            _currentLevel = level;

            if (_levelsMap.TryGetValue(_currentLevel, out var currentData))
            {
                CurrentLevelData = currentData;
            }
            else
            {
                CurrentLevelData = null;

                if (_currentLevel > 0)
                {
                    Debug.LogError($"Не найдены параметры для текущего {_currentLevel} уровня Аквариума!");
                }
            }

            _levelsMap.TryGetValue(_currentLevel + 1, out var nextData);

            NextLevelData = nextData;

            if (NextLevelData == null)
            {
                Debug.Log($"Достигнут макс. уровень или нет данных для {_currentLevel + 1} уровня.");
            }
        }

        public UpgradesConfig.LevelData GetLevelData(int level) => _levelsMap.GetValueOrDefault(level);

        public void UpgradeLevel()
        {
            SetLevel(_currentLevel + 1);
            
            OnAquariumUpgrade?.Invoke(CurrentLevelData);
        }

        public void Init()
        {
            _levelsMap = _upgradesConfig.LevelsUpgrades.ToDictionary(x => x.Level, x => x);

            SetLevel(0);
        }
    }
}