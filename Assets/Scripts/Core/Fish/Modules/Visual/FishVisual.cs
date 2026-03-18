using UnityEngine;

namespace Core.Fish.Modules.Visual
{
    public class FishVisual
    {
        private readonly FishEntity _fishEntity;
        private readonly Transform _visualTransform;

        public void UpdateVisuals()
        {
            if (_fishEntity.Movement.Velocity.x > 0.3f)
                _visualTransform.localScale = new Vector3(-1 * Mathf.Abs(_visualTransform.localScale.x), _visualTransform.localScale.y,
                    _visualTransform.localScale.z);
            else if (_fishEntity.Movement.Velocity.x < -0.3f)
                _visualTransform.localScale = new Vector3(1 * Mathf.Abs(_visualTransform.localScale.x), _visualTransform.localScale.y,
                    _visualTransform.localScale.z);

            var targetAngleZ = Mathf.Clamp(
                (_fishEntity.Movement.Velocity.y / _fishEntity.Config.SpeedRange.y) * _fishEntity.Config.MaxTiltAngle,
                -_fishEntity.Config.MaxTiltAngle,
                _fishEntity.Config.MaxTiltAngle);

            var tiltDirection = -_visualTransform.localScale.x;
            var targetRotation = Quaternion.Euler(0, 0, targetAngleZ * tiltDirection);

            _visualTransform.localRotation = Quaternion.Slerp(
                _visualTransform.localRotation,
                targetRotation,
                _fishEntity.Config.SteerSpeed * Time.fixedDeltaTime
            );
        }

        public FishVisual (FishEntity fishEntity, Transform visualTransform)
        {
            _fishEntity = fishEntity;
            _visualTransform = visualTransform;
        }
    }
}