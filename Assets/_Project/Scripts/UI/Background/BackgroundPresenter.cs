using System;
using _Project.Core.Interfaces;
using Core.Game.Upgrade;
using UnityEngine;

namespace UI.Background
{
    public class BackgroundPresenter : IUIInit, IDisposable
    {
        private readonly BackgroundView _view;
        private readonly UpgradeManager _upgradeManager;
        private readonly UpgradesConfig _config;

        public BackgroundPresenter(BackgroundView view, UpgradeManager upgradeManager, UpgradesConfig config)
        {
            _view = view;
            _upgradeManager = upgradeManager;
            _config = config;
        }

        private void HandleUpgrade(UpgradesConfig.LevelData levelData)
        {
            UpdateScale(animate: true);
        }

        private void UpdateScale(bool animate)
        {
            var maxLevelIndex = Mathf.Max(1, _config.TotalLevels);
            var progress = (float)_upgradeManager.CurrentLevel / maxLevelIndex;
            var targetScale = Mathf.Lerp(_config.InitialBackgroundScale, 1f, progress);

            _view.SetScale(targetScale, animate);
        }

        public void Init()
        {
            _upgradeManager.OnAquariumUpgrade += HandleUpgrade;

            _view.Init();

            UpdateScale(false);
        }

        public void Dispose()
        {
            _upgradeManager.OnAquariumUpgrade -= HandleUpgrade;
        }
    }
}