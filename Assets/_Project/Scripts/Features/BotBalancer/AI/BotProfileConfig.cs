using UnityEngine;

namespace Features.BotBalancer.AI
{
    public class BotProfileConfig : ScriptableObject
    {
        [Header("Настройки мышления")]
        [Tooltip("Как часто бот принимает решения (в игровых секундах)")]
        public float DecisionInterval = 1f;

        public float FeedFishesWeight = 0.8f;
        public float BuyFishWeight = 0.6f;
        
        public float CollectCoinsWeight = 0.9f;
        public float UpgradeAquariumWeight = 0.7f;
        
        [Tooltip("Приоритет уборки мертвых рыб")]
        public float CleanDeadFishWeight = 1.0f;
    }
}