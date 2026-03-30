using System;

namespace Core.Game
{
    public class BalanceManager
    {
        public event Action<int> OnCoinsCountChanged;

        public int CurrentCoinsCount { get; private set; }

        public BalanceManager(AquariumConfig config)
        {
            CurrentCoinsCount = config.StartCoins;
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
    }
}