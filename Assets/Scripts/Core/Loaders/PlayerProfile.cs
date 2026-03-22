using System;
using UnityEngine;

namespace Core.Profile
{
    public class PlayerProfile
    {
        private readonly AquariumConfig _aquariumConfig;

        public event Action<int> OnCoinsChanged;

        public int Coins { get; private set; }
        public int CurrentFishCount { get; private set; }
        
        public PlayerProfile(AquariumConfig aquariumConfig, ConfigValidator validator)
        {
            _aquariumConfig = aquariumConfig;
            
            LoadProfile();
        }

        private void LoadProfile()
        {
            Coins = _aquariumConfig.StartCoins;
            CurrentFishCount = 0;
            
            Debug.Log($"[PlayerProfile] Профиль загружен. Монеты: {Coins}");
        }

        public bool TrySpendCoins(int amount)
        {
            if (Coins < amount) return false;
            
            Coins -= amount;
            OnCoinsChanged?.Invoke(Coins);
            return true;
        }

        public void AddCoins(int amount)
        {
            Coins += amount;
            OnCoinsChanged?.Invoke(Coins);
        }

        public void RegisterFishSpawn() => CurrentFishCount++;
        public void RegisterFishDeath() => CurrentFishCount--;
        
        public bool CanAddFish() => CurrentFishCount < _aquariumConfig.MaxFishCount;
    }
}