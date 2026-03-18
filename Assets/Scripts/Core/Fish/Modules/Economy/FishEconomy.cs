using UnityEngine;

public class FishEconomy
{
    private readonly FishEntity _fish;
    
    private float _coinTimer;

    public FishEconomy(FishEntity fish)
    {
        _fish = fish;
        _coinTimer = 0f;
    }

    private void SpawnCoin()
    {
        Debug.Log($"Рыбка принесла {_fish.Config.IncomeCoins} монет");
    }

    public void Tick(float deltaTime)
    {
        _coinTimer += deltaTime;

        if (_coinTimer >= _fish.Config.IncomeCooldownSeconds)
        {
            _coinTimer -= _fish.Config.IncomeCooldownSeconds;

            SpawnCoin();
        }
    }
}