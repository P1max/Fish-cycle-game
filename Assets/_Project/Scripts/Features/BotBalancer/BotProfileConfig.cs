using Sirenix.OdinInspector;
using UnityEngine;

namespace Features.BotBalancer.AI
{
    public class BotProfileConfig : ScriptableObject
    {
        [Title("Настройки мышления")]
        [LabelText("Интервал принятия решений (сек)")]
        [InfoBox("Как часто бот принимает решения (в игровых секундах)")]
        [MinValue(0.1f)]
        public float DecisionInterval = 1f;

        [BoxGroup("Веса действий", CenterLabel = true)]
        [LabelText("Кормление рыб")]
        [PropertyRange(0f, 1f)]
        public float FeedFishesWeight = 0.8f;

        [BoxGroup("Веса действий")]
        [LabelText("Покупка рыб")]
        [PropertyRange(0f, 1f)]
        public float BuyFishWeight = 0.6f;

        [BoxGroup("Веса действий")]
        [LabelText("Сбор монет")]
        [PropertyRange(0f, 1f)]
        public float CollectCoinsWeight = 0.9f;

        [BoxGroup("Веса действий")]
        [LabelText("Улучшение аквариума")]
        [PropertyRange(0f, 1f)]
        public float UpgradeAquariumWeight = 0.7f;

        [BoxGroup("Веса действий")]
        [LabelText("Уборка мертвых рыб")]
        [PropertyTooltip("Имеет приоритет уборки")]
        [PropertyRange(0f, 1f)]
        public float CleanDeadFishWeight = 1.0f;
    }
}