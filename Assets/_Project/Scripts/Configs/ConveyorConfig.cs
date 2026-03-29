using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Configs
{
    public class ConveyorConfig : ScriptableObject, IValidatableConfig
    {
        [BoxGroup("Настройки ленты")]
        [JsonProperty("conveyerSpeed")]
        [LabelText("Скорость движения"), PropertyRange(0.1f, 1000f)]
        public float ConveyorSpeed = 150f;

        [BoxGroup("Настройки ленты")]
        [JsonIgnore]
        [LabelText("Отступ (Item Spacing)")]
        public float ItemSpacing = 250f;

        [BoxGroup("Качество: Бесплатные")]
        [JsonProperty("freeFishQuality")]
        [LabelText("Диапазон качества (Бесплатный)")]
        public Vector2 FreeQualityRange = new(0.3f, 0.7f);

        [BoxGroup("Качество: Бесплатные")]
        [LabelText("Количество коинов для бесплатной рыбы")]
        public int CoinsForFreeFish = 100;
        
        [BoxGroup("Качество: Бесплатные")]
        [LabelText("Количество рыб в аквариуме для бесплатной рыбы")]
        public int FishesCountForFreeFish = 1;

        [BoxGroup("Качество: Улучшенные")]
        [LabelText("Кривая качества от количества монет")]
        public AnimationCurve QualityCurve = new(
            new Keyframe(0f, 1f),
            new Keyframe(800f, 1.2f),
            new Keyframe(2000f, 1.2f),
            new Keyframe(2001f, 1.7f),
            new Keyframe(8000f, 2.5f),
            new Keyframe(8001, 2.5f),
            new Keyframe(50000, 3f)
        );

        private void OnValidate()
        {
            ValidateData();
        }

        public void ValidateData()
        {
            ConveyorSpeed = Mathf.Max(0.1f, ConveyorSpeed);
            ItemSpacing = Mathf.Max(0f, ItemSpacing);

            FreeQualityRange.x = Mathf.Max(0.1f, FreeQualityRange.x);
            FreeQualityRange.y = Mathf.Max(FreeQualityRange.x, FreeQualityRange.y);
        }
    }
}