using Spawners;

public class FishEconomy
{
    private readonly FishEntity _fish;
    private readonly CoinsPool _coinsPool;

    private float _coinTimer;

    public FishEconomy(FishEntity fish, CoinsPool coinsPool)
    {
        _fish = fish;
        _coinsPool = coinsPool;
        _coinTimer = 0f;
    }

    private void SpawnCoin()
    {
        _coinsPool.GetCoin(_fish.transform.position, _fish.Config.IncomeCoins);
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