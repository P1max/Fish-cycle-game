using Sirenix.OdinInspector;
using UnityEngine;

namespace Features.BotBalancer.AI
{
    [CreateAssetMenu(fileName = "BotProfile", menuName = "Bot/BotProfile")]
    public class BotProfileConfig : ScriptableObject
    {
        [Title("Настройки мышления")]
        [LabelWidth(250)]
        [LabelText("Интервал принятия решений (сек)")]
        [MinValue(0.1f)]
        public float DecisionInterval = 1f;

        [Title("Экономика и Замещение")]
        [LabelWidth(250)]
        [LabelText("Жадность (множитель замены)")]
        [InfoBox("Во сколько раз новая рыба должна быть выгоднее старой, чтобы бот начал морить старых голодом? (1.2 = на 20% лучше).")]
        [MinValue(1.0f)]
        public float UpgradeThreshold = 1.2f;

        [LabelWidth(250)]
        [LabelText("Морить голодом")]
        [Tooltip("Бот не будет кормить рыб, если есть цель заморить их голодом и купить рыбу лучше.")]
        public bool UseStarvationStrategy = true;
        
        [LabelWidth(250)]
        [LabelText("Множитель веса при голодании")]
        [Tooltip("На сколько умножается вес кормежки, если бот решил морить рыбу. 0 - абсолютная смерть, 0.1 - покормит, если совсем нечего делать.")]
        [PropertyRange(0f, 1f)]
        [ShowIf("UseStarvationStrategy")]
        public float StarvationWeightMultiplier = 0f;

        [Title("Стратегия «Шлюз» (Трупы-блокаторы)")]
        [LabelWidth(250)]
        [LabelText("Трупы как блокировщики")]
        [Tooltip("Бот не будет убирать трупы, пока не накопит на замену, чтобы рыбы не размножались.")]
        public bool UseAirlockStrategy = true;

        [LabelWidth(250)]
        [LabelText("Множитель веса уборки")]
        [Tooltip("На сколько умножается вес уборки трупов, если замена еще не найдена. 0 - будет хранить трупы вечно.")]
        [PropertyRange(0f, 1f)]
        [ShowIf("UseAirlockStrategy")]
        public float AirlockWeightMultiplier = 0f;

        [BoxGroup("Веса действий", CenterLabel = true)] 
        [PropertyRange(0f, 1f)] 
        public float FeedFishesWeight = 0.8f;
        
        [BoxGroup("Веса действий")] 
        [PropertyRange(0f, 1f)] 
        public float BuyFishWeight = 0.6f;
        
        [BoxGroup("Веса действий")] 
        [PropertyRange(0f, 1f)] 
        public float CollectCoinsWeight = 0.9f;
        
        [BoxGroup("Веса действий")] 
        [PropertyRange(0f, 1f)] 
        public float UpgradeAquariumWeight = 0.7f;
        
        [BoxGroup("Веса действий")] 
        [PropertyRange(0f, 1f)] 
        public float CleanDeadFishWeight = 1.0f;
    }
}