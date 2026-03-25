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

        [BoxGroup("Качество: Бесплатные (Баланс 0)")]
        [JsonProperty("freeFishQuality")]
        [LabelText("Диапазон качества (0 монет)")]
        public Vector2 FreeQualityRange = new(0.3f, 0.7f);

        [BoxGroup("Качество: Обычные")]
        [JsonProperty("defultFishQuality")]
        [LabelText("Базовый множитель")]
        public float DefaultFishQuality = 1f;

        [BoxGroup("Качество: Обычные")]
        [JsonProperty("defultQualityCoinsRange")]
        [LabelText("Диапазон монет (Базовый)")]
        public Vector2 DefaultQualityCoinsRange = new(100, 500);

        [BoxGroup("Качество: Улучшенные")]
        [JsonProperty("upgradeFishQualityRange")]
        [LabelText("Диапазон множителя (Улучшенный)")]
        public Vector2 UpgradeFishQualityRange = new(2, 8);

        [BoxGroup("Качество: Улучшенные")]
        [JsonProperty("upgradeQualityCoinsRange")]
        [LabelText("Диапазон монет (Улучшенный)")]
        public Vector2 UpgradeQualityCoinsRange = new(500, 50000);

        public void ValidateData()
        {
            ConveyorSpeed = Mathf.Max(0.1f, ConveyorSpeed);
        }
    }
}