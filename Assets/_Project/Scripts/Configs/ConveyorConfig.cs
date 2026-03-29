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

        private void OnValidate()
        {
            ValidateData();
        }

        public void ValidateData()
        {
            ConveyorSpeed = Mathf.Max(0.1f, ConveyorSpeed);
            ItemSpacing = Mathf.Max(0f, ItemSpacing);
            DefaultFishQuality = Mathf.Max(0.1f, DefaultFishQuality);

            FreeQualityRange.x = Mathf.Max(0.1f, FreeQualityRange.x);
            FreeQualityRange.y = Mathf.Max(FreeQualityRange.x, FreeQualityRange.y);

            DefaultQualityCoinsRange.x = Mathf.Max(1f, DefaultQualityCoinsRange.x);
            DefaultQualityCoinsRange.y = Mathf.Max(DefaultQualityCoinsRange.x + 1f, DefaultQualityCoinsRange.y);

            UpgradeQualityCoinsRange.x = DefaultQualityCoinsRange.y; 
            UpgradeQualityCoinsRange.y = Mathf.Max(UpgradeQualityCoinsRange.x + 1f, UpgradeQualityCoinsRange.y);
            
            UpgradeFishQualityRange.x = Mathf.Max(DefaultFishQuality, UpgradeFishQualityRange.x);
            UpgradeFishQualityRange.y = Mathf.Max(UpgradeFishQualityRange.x, UpgradeFishQualityRange.y);
        }
    }
}