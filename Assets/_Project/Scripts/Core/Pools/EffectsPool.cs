using Core.Entities.VFX;
using UnityEngine;
using Zenject;

namespace Spawners
{
    public class EffectsPool : BaseEntityPool<VFXEntity>
    {
        [Inject] private AquariumConfig _aquariumConfig;

        protected override void Awake()
        {
            base.Awake();
            
            transform.localScale =  Vector3.one * _aquariumConfig.DefaultEntitiesScale * _aquariumConfig.FishesDefaultScale;
        }

        protected override void LoadPrefab()
        {
            Prefab = Resources.Load<VFXEntity>("Prefabs/VFX_Birth");
        }

        protected override void OnItemCreated(VFXEntity item)
        {
            item.Init(this);
        }

        public void SpawnEffect(Vector2 position)
        {
            var effect = Get();
;            
            effect.Play(position);
        }
    }
}