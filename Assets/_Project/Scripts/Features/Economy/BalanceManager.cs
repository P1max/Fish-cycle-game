using System;
using _Project.Core.Interfaces;

namespace Core.Game
{
    public class BalanceManager : IGameplayInit
    {
        private readonly AquariumConfig _config;
        
        public event Action<int> OnCoinsCountChanged;

        public int CurrentCoinsCount { get; private set; }

        public BalanceManager(AquariumConfig config)
        {
            _config = config;
        }

        public void AddCoins(int amount)
        {
            CurrentCoinsCount += amount;
            
            OnCoinsCountChanged?.Invoke(CurrentCoinsCount);
        }

        public bool TrySpendCoins(int amount)
        {
            if (CurrentCoinsCount < amount) return false;

            CurrentCoinsCount -= amount;
            
            OnCoinsCountChanged?.Invoke(CurrentCoinsCount);

            return true;
        }

        public void Init()
        {
            CurrentCoinsCount = _config.StartCoins;
        }
    }
}