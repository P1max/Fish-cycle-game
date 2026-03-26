using Spawners;
using UnityEngine;

public class FishEconomy
{
    private readonly FishEntity _fish;
    private readonly CoinsPool _coinsPool;

    private float _coinTimer;

    public float TimeToNextCoin => Mathf.Max(0, _fish.Config.IncomeCooldownSeconds - _coinTimer);

    public FishEconomy(FishEntity fish, CoinsPool coinsPool)
    {
        _fish = fish;
        _coinsPool = coinsPool;
        _coinTimer = 0f;
    }

    private void SpawnCoin()
    {
        _coinsPool.SpawnCoin(_fish.transform.position, _fish.Config.IncomeCoins);
    }

    public void Reset()
    {
        _coinTimer = 0f;
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