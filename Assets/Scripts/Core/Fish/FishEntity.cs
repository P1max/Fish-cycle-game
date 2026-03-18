using UnityEngine;
using System;
using System.Collections.Generic;
using Core.Fish.Modules.Visual;
using Spawners;

public class FishEntity : MonoBehaviour
{
    [SerializeField] private Transform _visualTransform;

    private bool _isActive;

    public event Action<FishEntity> OnDeath;

    public FishConfig Config { get; private set; }
    public FishMovement Movement { get; private set; }
    public FishHunger Hunger { get; private set; }
    public FishEconomy Economy { get; private set; }
    public FishLifeCycle LifeCycle { get; private set; }
    public FishVisual FishVisual { get; private set; }
    
    private void Update()
    {
        if (!_isActive) return;

        var delta = Time.deltaTime;

        Hunger.Tick(delta);
        Economy.Tick(delta);
        LifeCycle.Tick(delta);
    }

    private void FixedUpdate()
    {
        if (!_isActive) return;

        Movement.Tick();
    }

    public void Die()
    {
        _isActive = false;
        Movement.Stop();
        OnDeath?.Invoke(this);

        Debug.Log($"Рыбка умерла");
    }

    public void Init(FishConfig config, IReadOnlyDictionary<Collider2D, FishEntity> fishesCache, FoodPool foodPool)
    {
        Config = config;

        if (!Config) Debug.LogWarning("Рыбе не назначен конфиг.");

        Hunger = new FishHunger(this);
        Economy = new FishEconomy(this);
        LifeCycle = new FishLifeCycle(this);
        Movement = new FishMovement(this, fishesCache, foodPool, GetComponent<Rigidbody2D>());
        FishVisual = new FishVisual(this, _visualTransform);

        _isActive = true;
    }
}