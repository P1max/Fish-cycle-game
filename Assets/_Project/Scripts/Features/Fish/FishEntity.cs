using System;
using UnityEngine;
using System.Collections.Generic;
using Core.Entities.Fish.Modules.Breeding;
using Core.Fish.Modules;
using Core.Fish.Modules.Visual;
using Core.Game;
using Core.Loaders;
using Spawners;
using DG.Tweening;
using Random = UnityEngine.Random;

[RequireComponent(typeof(FishVisual))]
public class FishEntity : MonoBehaviour
{
    private FishesConfigsLoader _fishesConfigsLoader;
    private Collider2D _collider;
    private bool _isAlive;
    private bool _isCollecting;

    public event Action<FishEntity> OnReadyToPool;
    public event Action<FishEntity> OnReturnToPool;

    public bool IsAlive => _isAlive;

    public FishConfig Config { get; private set; }
    public CommonFishConfig CommonFishConfig { get; private set; }
    public FishMovement Movement { get; private set; }
    public FishHunger Hunger { get; private set; }
    public FishEconomy Economy { get; private set; }
    public FishLifeCycle LifeCycle { get; private set; }
    public FishVisual FishVisual { get; private set; }
    public FishScanner Scanner { get; private set; }
    public FishBreeding Breeding { get; private set; }
    public FishIndicator Indicator { get; private set; }
    public float BaseSpeed { get; private set; }

    private void Awake()
    {
        FishVisual = GetComponent<FishVisual>();
    }

    private void Update()
    {
        if (!_isAlive) return;

        var delta = Time.deltaTime;

        Hunger.Tick(delta);
        Economy.Tick(delta);
        LifeCycle.Tick(delta);
        Breeding.Tick(delta);
        Indicator.Tick();
    }

    private void FixedUpdate()
    {
        if (!_isAlive) return;

        Scanner.Tick();
        Movement.Tick();
    }

    private void OnDrawGizmosSelected()
    {
        if (Config == null || CommonFishConfig == null) return;

        var currentPosition = transform.position;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(currentPosition, CommonFishConfig.NeighborRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(currentPosition, CommonFishConfig.SeparationRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(currentPosition, CommonFishConfig.FoodSearchRadius);

        if (Movement != null && IsAlive)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(currentPosition, Movement.Velocity);
        }
    }

    private void Refresh()
    {
        name = Config.Id;
        _isAlive = true;
        _isCollecting = false;
        _collider.enabled = true;

        Hunger.Reset();
        Economy.Reset();
        LifeCycle.Reset();
        Breeding.Reset();

        FishVisual.ResetVisuals();
        FishVisual.SetSprite(Config.Sprite);

        Movement.Start();
    }

    public void Die()
    {
        _isAlive = false;
        Movement.Stop();
        FishVisual.SetDeadVisuals();
        Indicator.HideDeath();
        Indicator.HideHungry();
    }

    public void Collect()
    {
        if (_isAlive || _isCollecting) return;

        _isCollecting = true;

        DOVirtual.DelayedCall(0.01f, () =>
        {
            if (this != null && _collider != null)
            {
                _collider.enabled = false;
            }
        });

        OnReadyToPool?.Invoke(this);

        FishVisual.PlayCollectAnimation(() => OnReturnToPool?.Invoke(this));
    }

    public void SetConfig(string fishId, float quality)
    {
        var baseConfig = _fishesConfigsLoader.LoadedFishesDict[fishId];

        Config = baseConfig.Clone();
        Config.LifetimeSeconds *= quality;
        Config.IncomeCoins = Mathf.RoundToInt(Config.IncomeCoins * quality);
        Config.Price = Mathf.RoundToInt(Config.Price * quality);

        Refresh();

        var randomSize = Random.Range(Config.SizeModifier.x, Config.SizeModifier.y);
        var targetScale = Vector3.one * randomSize;

        BaseSpeed = Random.Range(Config.NormalSpeedRange.x, Config.NormalSpeedRange.y);

        FishVisual.PlaySpawnAnimation(targetScale);
    }

    public void Init(IReadOnlyDictionary<Collider2D, FishEntity> fishesCache, FoodPool foodPool, Collider2D thisCollider,
        CommonFishConfig commonFishConfig, CoinsPool coinsPool, AquariumBoundsManager aquariumBoundsManager, BreedManager breedManager,
        FishesConfigsLoader fishesConfigsLoader)
    {
        _collider = thisCollider;
        CommonFishConfig = commonFishConfig;
        _fishesConfigsLoader = fishesConfigsLoader;

        Hunger = new FishHunger(this);
        Economy = new FishEconomy(this, coinsPool);
        LifeCycle = new FishLifeCycle(this);

        Movement = new FishMovement(this, GetComponent<Rigidbody2D>(), _collider,
            aquariumBoundsManager);

        FishVisual.Init(this);

        Scanner = new FishScanner(this, fishesCache, foodPool, coinsPool);
        Breeding = new FishBreeding(this, breedManager);
        Indicator = new FishIndicator(this, FishVisual.HungryContainer, FishVisual.DeathContainer, FishVisual.DeathText, FishVisual.DeathPulseCircle);

        _isAlive = true;
    }
}