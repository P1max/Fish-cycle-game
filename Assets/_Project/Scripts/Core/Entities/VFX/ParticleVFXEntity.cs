using UnityEngine;
using Spawners;

namespace Core.Entities.VFX
{
    public class ParticleVFXEntity : BaseVFXEntity
    {
        [SerializeField] private ParticleSystem _particleSystem;

        public override void Init(EffectsPool pool)
        {
            base.Init(pool);

            var main = _particleSystem.main;

            main.stopAction = ParticleSystemStopAction.Callback;
        }

        public override void Play(Vector2 position)
        {
            transform.position = position;
            _particleSystem.Play();
        }

        private void OnParticleSystemStopped()
        {
            ReturnToPool();
        }
    }
}