using System;
using UnityEngine;
using System.Collections.Generic;
using Core.Entities.Fish.Modules.Breeding;
using Core.Fish.Modules;
using Core.Fish.Modules.Visual;
using Core.Game;
using Core.Loaders;
using Spawners;
using TMPro;
using Random = UnityEngine.Random;

public class FishEntity : MonoBehaviour
{
    [SerializeField] private Transform _visualTransform;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    
    [Header("Indicator Settings")]
    [SerializeField] private GameObject _hungryContainer;
    [SerializeField] private GameObject _deathContainer;
    [SerializeField] private TextMeshPro _deathText;

    private FishesLoader _fishesLoader;
    private Collider2D _collider;
    private bool _isAlive;

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
        if (_isAlive) return;

        _collider.enabled = false;

        OnReadyToPool?.Invoke(this);

        FishVisual.PlayCollectAnimation(() => OnReturnToPool?.Invoke(this));
    }

    public void SetConfig(string fishId, float quality)
    {
        var baseConfig = _fishesLoader.LoadedFishesDict[fishId];
        
        Config = baseConfig.Clone();
        Config.LifetimeSeconds *= quality;
        Config.IncomeCoins = Mathf.RoundToInt(Config.IncomeCoins * quality);
        Config.Price = Mathf.RoundToInt(Config.Price * quality);
        
        Refresh();
        
        var randomSize = Random.Range(Config.SizeModifier.x, Config.SizeModifier.y);

        transform.localScale = new Vector3(randomSize, randomSize, 1f);
        
        BaseSpeed = Random.Range(Config.NormalSpeedRange.x, Config.NormalSpeedRange.y);
    }

    public void Init(IReadOnlyDictionary<Collider2D, FishEntity> fishesCache, FoodPool foodPool, Collider2D thisCollider,
        CommonFishConfig commonFishConfig, CoinsPool coinsPool, AquariumBoundsManager aquariumBoundsManager, BreedManager breedManager,
        FishesLoader fishesLoader)
    {
        _collider = thisCollider;
        CommonFishConfig = commonFishConfig;
        _fishesLoader = fishesLoader;

        Hunger = new FishHunger(this);
        Economy = new FishEconomy(this, coinsPool);
        LifeCycle = new FishLifeCycle(this);

        Movement = new FishMovement(this, GetComponent<Rigidbody2D>(), _collider,
            aquariumBoundsManager);

        FishVisual = new FishVisual(this, _visualTransform, _spriteRenderer);
        Scanner = new FishScanner(this, fishesCache, foodPool, coinsPool);
        Breeding = new FishBreeding(this, breedManager);
        Indicator = new FishIndicator(this, _hungryContainer, _deathContainer, _deathText);

        _isAlive = true;
    }
}