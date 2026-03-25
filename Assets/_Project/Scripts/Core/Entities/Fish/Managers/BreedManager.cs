using Spawners;
using UnityEngine;

namespace Core.Game
{
    public class BreedManager
    {
        private readonly FishesManager _fishesManager;
        private readonly EffectsPool _effectsPool;

        public BreedManager(FishesManager fishesManager, EffectsPool effectsPool)
        {
            _fishesManager = fishesManager;
            _effectsPool = effectsPool;
        }

        private void SpawnBirthParticles(FishEntity firstFish, FishEntity secondFish)
        {
            _effectsPool.SpawnEffect(firstFish.transform.position);
            _effectsPool.SpawnEffect(secondFish.transform.position);
        }

        public void TryBreed(FishEntity firstFish, FishEntity secondFish)
        {
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

                SpawnBirthParticles(firstFish, secondFish);

                _fishesManager.TryAddFish(childFishId, spawnPosition: spawnPos);
            }
        }
    }
}