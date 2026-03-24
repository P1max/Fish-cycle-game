using UnityEngine;

public class FishHunger
{
    private readonly FishEntity _fish;

    public float CurrentHungerPercent { get; private set; }

    public FishHunger(FishEntity fish)
    {
        _fish = fish;
        CurrentHungerPercent = 0f;
    }

    public void Reset()
    {
        CurrentHungerPercent = 0f;
    }

    public void Feed(float nutritionValue)
    {
        CurrentHungerPercent -= nutritionValue;
        CurrentHungerPercent = Mathf.Max(CurrentHungerPercent, 0f);
    }

    public void Tick(float deltaTime)
    {
        CurrentHungerPercent += _fish.Config.HungerGrowthPercentPerSecond * deltaTime;

        CurrentHungerPercent = Mathf.Clamp(CurrentHungerPercent, 0f, 100f);

        if (CurrentHungerPercent >= 100f)
        {
            _fish.Die();
        }
    }
}