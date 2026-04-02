using System;
using System.IO;
using _Project.Core.Interfaces;
using Core.Feed;
using Core.Game;
using UnityEngine;
using Zenject;

namespace Features.BotBalancer.Analytics
{
    public class BotAnalyticsService : IGameplayInit, ITickable, IDisposable
    {
        private readonly BotSetupService _botSetup;
        private readonly BalanceManager _balanceManager;
        private readonly FishesManager _aquarium;
        private readonly StoreManager _storeManager;
        private readonly BreedManager _breedManager;
        private readonly FeedManager _feedManager;

        private SimulationReport _report;
        private float _sessionTimer;
        private float _reportsInterval;
        private float _nextSnapshotTime;
        private int _targetSimulationTime;
        private int _totalDead;
        private int _totalBought;
        private int _totalBorn;
        private int _totalFeederUsed;
        private int _totalCoinsCollected;

        private bool _isReportSaved;
        private string _reportFilePath;

        public BotAnalyticsService(
            BotSetupService botSetup,
            BalanceManager balanceManager,
            FishesManager aquarium,
            StoreManager storeManager,
            BreedManager breedManager,
            FeedManager feedManager)
        {
            _botSetup = botSetup;
            _balanceManager = balanceManager;
            _aquarium = aquarium;
            _storeManager = storeManager;
            _breedManager = breedManager;
            _feedManager = feedManager;
        }

        private void HandleFishBought() => _totalBought++;

        private void HandleFishBorn() => _totalBorn++;

        private void HandleFishRemoved() => _totalDead++;

        private void HandleFeederUsed() => _totalFeederUsed++;

        private void HandleCoinsAdded(int amount) => _totalCoinsCollected += amount;

        private void RecordSnapshot(float timeStamp)
        {
            var snapshot = new IntervalSnapshot
            {
                TimeSecond = timeStamp,
                CoinsBalance = _balanceManager.CurrentCoinsCount,
                FishesCount = _aquarium.CurrentFishCount,
                DeadCount = _totalDead,
                BoughtCount = _totalBought,
                BornCount = _totalBorn,
                FeederUsedCount = _totalFeederUsed,
                CoinsCollectedAmount = _totalCoinsCollected
            };

            _report.Snapshots.Add(snapshot);
        }

        private void SaveReport()
        {
            if (_isReportSaved || _report == null || _report.Snapshots.Count == 0) return;

            _isReportSaved = true;

            try
            {
                var folderPath = Path.GetDirectoryName(_reportFilePath);

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                Debug.Log($"Сохранение отчета... Собрано {_report.Snapshots.Count} записей.");

                var json = JsonUtility.ToJson(_report, true);

                File.WriteAllText(_reportFilePath, json);

                Debug.Log($"Отчет успешно сохранен: {_reportFilePath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Ошибка при сохранении отчета: {e.Message}");
            }
        }

        private void FinishSimulation()
        {
            Debug.Log("Симуляция завершилась.");

            SaveReport();

#if UNITY_EDITOR
            UnityEditor.EditorPrefs.SetString("AutoOpenReportPath", _reportFilePath);

            if (UnityEditor.EditorApplication.isPlaying)
            {
                UnityEditor.EditorApplication.isPlaying = false;
            }
#endif
        }

        public void Tick()
        {
            if (!_botSetup.IsBotActive) return;

            _sessionTimer += Time.deltaTime;

            while (_sessionTimer >= _nextSnapshotTime)
            {
                RecordSnapshot(_nextSnapshotTime);

                _nextSnapshotTime += _reportsInterval;

                if (_reportsInterval <= 0f) break;
            }

            if (_sessionTimer >= _targetSimulationTime)
            {
                if (_report.Snapshots.Count == 0 || Mathf.Abs(_report.Snapshots[^1].TimeSecond - _targetSimulationTime) > 0.01f)
                {
                    RecordSnapshot(_targetSimulationTime);
                }

                FinishSimulation();
            }
        }

        public void Dispose()
        {
            if (!_botSetup.IsBotActive) return;

            SaveReport();

            if (_storeManager != null) _storeManager.OnFishBought -= HandleFishBought;
            if (_breedManager != null) _breedManager.OnFishBorn -= HandleFishBorn;
            if (_aquarium != null) _aquarium.OnFishRemoved -= HandleFishRemoved;
            if (_feedManager != null) _feedManager.OnFeederUsed -= HandleFeederUsed;
            if (_balanceManager != null) _balanceManager.OnCoinsAdded -= HandleCoinsAdded;
        }

        public void Init()
        {
            if (!_botSetup.IsBotActive) return;

            _report = new SimulationReport();
            _isReportSaved = false;

            _sessionTimer = 0f;
            _totalDead = 0;
            _totalBought = 0;
            _totalBorn = 0;
            _totalFeederUsed = 0;
            _totalCoinsCollected = 0;

            var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            var folderPath = Path.Combine(Application.dataPath, "Reports");

            _reportFilePath = Path.Combine(folderPath, $"SimReport_{timestamp}.json");

#if UNITY_EDITOR
            _targetSimulationTime = UnityEditor.EditorPrefs.GetInt("Bot_SimTime", 60);
            _reportsInterval = UnityEditor.EditorPrefs.GetFloat("Bot_ReportsInterval", 1f);
#endif

            if (_reportsInterval <= 0f) _reportsInterval = 1f;

            _nextSnapshotTime = 0f;

            _storeManager.OnFishBought += HandleFishBought;
            _breedManager.OnFishBorn += HandleFishBorn;
            _aquarium.OnFishRemoved += HandleFishRemoved;
            _feedManager.OnFeederUsed += HandleFeederUsed;
            _balanceManager.OnCoinsAdded += HandleCoinsAdded;

            RecordSnapshot(0f);

            _nextSnapshotTime += _reportsInterval;
        }
    }
}