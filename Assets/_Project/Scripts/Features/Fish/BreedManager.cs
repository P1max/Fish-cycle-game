using System;
using _Project.Core.Interfaces;
using Spawners;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Core.Game
{
    public class BreedManager : IGameplayInit
    {
        private readonly FishesManager _fishesManager;
        private readonly EffectsPool _effectsPool;

        private bool _isInit;

        public event Action OnFishBorn;

        public BreedManager(FishesManager fishesManager, EffectsPool effectsPool)
        {
            _fishesManager = fishesManager;
            _effectsPool = effectsPool;
        }

        public void TryBreed(FishEntity firstFish, FishEntity secondFish)
        {
            if (!_isInit) return;

            if (!_fishesManager.CanAddFish) return;

            if (!firstFish.Breeding.IsReady || !secondFish.Breeding.IsReady) return;

            firstFish.Breeding.ResetCooldown();
            secondFish.Breeding.ResetCooldown();

            var totalChance = firstFish.Config.BreedChancePercent + secondFish.Config.BreedChancePercent;
            var roll = Random.Range(0f, 100f);

            if (roll <= totalChance)
            {
                Vector2 spawnPos = (firstFish.transform.position + secondFish.transform.position) / 2f;

                var childFishId = Random.value > 0.5f ? firstFish.Config.Id : secondFish.Config.Id;

                if (_fishesManager.TryAddFish(childFishId, spawnPosition: spawnPos))
                {
                    _effectsPool.SpawnEffect("birth", spawnPos);

                    OnFishBorn?.Invoke();
                }
            }
        }

        public void Init()
        {
            _isInit = true;
        }
    }
}