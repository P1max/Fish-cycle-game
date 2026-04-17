using Spawners;
using UnityEngine;

public class FishEconomy
{
    private readonly FishEntity _fish;
    private readonly CoinsPool _coinsPool;

    private float _coinTimer;
    private bool _isPooping;

    public float TimeToNextCoin => Mathf.Max(0, _fish.Config.IncomeCooldownSeconds - _coinTimer);

    public FishEconomy(FishEntity fish, CoinsPool coinsPool)
    {
        _fish = fish;
        _coinsPool = coinsPool;
        _coinTimer = 0f;
        _isPooping = false;
    }

    private void SpawnCoin()
    {
        var offsetDistance = 0.5f;
        var direction = Mathf.Sign(_fish.FishVisual.VisualTransform.localScale.x);
        var spawnOffset = new Vector3(direction * offsetDistance, 0, 0);
        var spawnPos = _fish.transform.position + spawnOffset;

        _coinsPool.SpawnCoin(spawnPos, _fish.Config.IncomeCoins);
    }

    public void Reset()
    {
        _coinTimer = 0f;
        _isPooping = false;
    }

    public void Tick(float deltaTime)
    {
        _coinTimer += deltaTime;

        var timeToSpawn = _fish.Config.IncomeCooldownSeconds - _coinTimer;

        if (timeToSpawn <= _fish.FishVisual.PoopAnimPreDelay && !_isPooping)
        {
            _isPooping = true;
            _fish.FishVisual.PlayPoopCoinAnimation(SpawnCoin);
        }

        if (_coinTimer >= _fish.Config.IncomeCooldownSeconds)
        {
            _coinTimer -= _fish.Config.IncomeCooldownSeconds;
            _isPooping = false;
        }
    }
}