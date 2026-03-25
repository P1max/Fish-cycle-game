public class FishLifeCycle
{
    private readonly FishEntity _fish;

    private float _lifeTimer;

    public FishLifeCycle(FishEntity fish)
    {
        _fish = fish;
    }

    public void Reset()
    {
        _lifeTimer = 0f;
    }

    public void Tick(float deltaTime)
    {
        _lifeTimer += deltaTime;

        if (_lifeTimer >= _fish.Config.LifetimeSeconds)
        {
            _lifeTimer = 0f;

            _fish.Die();
        }
    }
}