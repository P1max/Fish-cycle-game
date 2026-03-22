using System;
using UnityEngine;
using System.Collections.Generic;
using Core.Fish.Modules.Visual;
using Spawners;
using Random = UnityEngine.Random;

public class FishEntity : MonoBehaviour
{
    [SerializeField] private Transform _visualTransform;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private bool _isAlive;
    private Collider2D _collider;

    public event Action<FishEntity> OnReadyToPool;
    public event Action<FishEntity> OnReturnedToPool;

    public bool IsAlive => _isAlive;

    public FishConfig Config { get; private set; }
    public CommonFishConfig CommonFishConfig { get; private set; }
    public FishMovement Movement { get; private set; }
    public FishHunger Hunger { get; private set; }
    public FishEconomy Economy { get; private set; }
    public FishLifeCycle LifeCycle { get; private set; }
    public FishVisual FishVisual { get; private set; }
    public float BaseSpeed { get; private set; }

    private void Update()
    {
        if (!_isAlive) return;

        var delta = Time.deltaTime;

        Hunger.Tick(delta);
        Economy.Tick(delta);
        LifeCycle.Tick(delta);
    }

    private void FixedUpdate()
    {
        if (!_isAlive) return;

        Movement.Tick();
    }

    public void Die()
    {
        _isAlive = false;
        Movement.Stop();
        FishVisual.SetDeadVisuals();
    }

    public void Collect()
    {
        if (_isAlive) return;

        _collider.enabled = false;

        OnReadyToPool?.Invoke(this);

        FishVisual.PlayCollectAnimation(() => OnReturnedToPool?.Invoke(this));
    }

    public void SetConfig(FishConfig config)
    {
        Config = config;

        if (!Config) Debug.LogWarning("Конфиг рыбы пуст.");
        else
        {
            _collider.enabled = true;

            Hunger.Reset();
            Economy.Reset();
            LifeCycle.Reset();
            FishVisual.ResetVisuals();
            FishVisual.SetSprite(config.Sprite);

            var randomSize = Random.Range(config.SizeModifier.x, config.SizeModifier.y);

            transform.localScale = new Vector3(randomSize, randomSize, 1f);
            BaseSpeed = Random.Range(config.NormalSpeedRange.x, config.NormalSpeedRange.y);
        }
    }

    public void Init(IReadOnlyDictionary<Collider2D, FishEntity> fishesCache, FoodPool foodPool, Collider2D thisCollider,
        CommonFishConfig commonFishConfig, CoinsPool coinsPool)
    {
        _collider = thisCollider;
        CommonFishConfig = commonFishConfig;

        Hunger = new FishHunger(this);
        Economy = new FishEconomy(this, coinsPool);
        LifeCycle = new FishLifeCycle(this);
        Movement = new FishMovement(this, fishesCache, foodPool, GetComponent<Rigidbody2D>(), _collider);
        FishVisual = new FishVisual(this, _visualTransform, _spriteRenderer);

        _isAlive = true;
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
}