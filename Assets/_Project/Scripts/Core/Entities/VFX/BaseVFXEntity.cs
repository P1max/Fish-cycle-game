using UnityEngine;
using Spawners;

namespace Core.Entities.VFX
{
    public abstract class BaseVFXEntity : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private string _id;
        
        protected EffectsPool Pool;
        
        public string Id => _id;

        public virtual void Init(EffectsPool pool)
        {
            Pool = pool;
        }

        public abstract void Play(Vector2 position);

        protected void ReturnToPool()
        {
            Pool.ReturnToPool(this);
        }
    }
}