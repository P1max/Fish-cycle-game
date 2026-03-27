using Core.Entities.VFX;
using UnityEngine;

namespace Spawners
{
    public class EffectsPool : BaseEntityPool<VFXEntity>
    {
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