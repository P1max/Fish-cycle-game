using UnityEngine;
using System;
using System.Collections.Generic;

public class FishEntity : MonoBehaviour
{
    private bool _isActive;

    public event Action<FishEntity> OnDeath;

    public FishConfig Config { get; private set; }
    public FishMovement Movement { get; private set; }
    public FishHunger Hunger { get; private set; }
    public FishEconomy Economy { get; private set; }
    public FishLifeCycle LifeCycle { get; private set; }

    private void Awake()
    {
        Movement = GetComponent<FishMovement>();
    }

    private void Update()
    {
        if (!_isActive) return;

        var delta = Time.deltaTime;

        Hunger.Tick(delta);
        Economy.Tick(delta);
        LifeCycle.Tick(delta);
    }

    public void Die()
    {
        _isActive = false;
        OnDeath?.Invoke(this);
        
        Debug.Log($"Рыбка умерла");
    }

    public void Init(FishConfig config, IReadOnlyDictionary<Collider2D, FishEntity> fishesCache)
    {
        Config = config;
        
        Hunger = new FishHunger(this);
        Economy = new FishEconomy(this);
        LifeCycle = new FishLifeCycle(this);

        Movement.Init(this, fishesCache);

        _isActive = true;
    }
}