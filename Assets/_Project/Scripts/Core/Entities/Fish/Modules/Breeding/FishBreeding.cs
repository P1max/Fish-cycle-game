using Core.Game;
using UnityEngine;

namespace Core.Entities.Fish.Modules.Breeding
{
    public class FishBreeding
    {
        private readonly FishEntity _fish;
        private readonly BreedManager _breedManager;

        private float _cooldownTimer;

        public float TimeToBreed => Mathf.Max(0, _cooldownTimer);

        public bool IsReady => _cooldownTimer <= 0f;

        public FishBreeding(FishEntity fish, BreedManager breedManager)
        {
            _fish = fish;
            _breedManager = breedManager;
        }

        public void Reset() => ResetCooldown();

        public void ResetCooldown()
        {
            _cooldownTimer = _fish.Config.BreedCooldownSeconds;
        }

        public void Tick(float deltaTime)
        {
            if (!IsReady)
            {
                _cooldownTimer -= deltaTime;

                return;
            }

            TryFindPartner();
        }

        private void TryFindPartner()
        {
            foreach (var partner in _fish.Scanner.NearbyAliveFishes)
            {
                if (!partner.Breeding.IsReady) continue;

                var distance = Vector2.Distance(_fish.transform.position, partner.transform.position);

                if (distance > _fish.CommonFishConfig.PartnerSearchRadius) continue;

                _breedManager.TryBreed(_fish, partner);

                break;
            }
        }
    }
}