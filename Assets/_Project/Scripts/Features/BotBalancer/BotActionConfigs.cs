using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Features.BotBalancer.AI.Configs
{
    [Serializable]
    [GUIColor("@$value.IsEnabled ? Color.white : Color.gray")]
    public abstract class BotActionConfig
    {
        [VerticalGroup("Header")]
        [LabelWidth(250)]
        [LabelText("Включить экшен")]
        public bool IsEnabled = true;

        [VerticalGroup("Header")]
        [LabelWidth(250)]
        [LabelText("Базовый вес")]
        [PropertyRange(0f, 1f)]
        [ShowIf("IsEnabled")]
        public float BaseWeight = 1f;

        public abstract Type TargetActionType { get; }
    }

    [Serializable]
    [LabelText("Кормление рыб")]
    [Tooltip(
        "Как считается вес:\n• Плавно растет до Базового веса, если элитные рыбы начинают голодать.\n• Стратегия Голодания: Если аквариум забит и есть дешевые рыбы-мусор, бот специально перестанет их кормить, чтобы освободить место.")]
    public class FeedActionConfig : BotActionConfig
    {
        [ShowIf("IsEnabled")]
        [ToggleGroup("UseStarvationStrategy", "Стратегия Голодания")]
        public bool UseStarvationStrategy = false;

        [ShowIf("IsEnabled")]
        [ToggleGroup("UseStarvationStrategy")]
        [LabelWidth(250)]
        [LabelText("Жадность (множитель замены)")]
        public float UpgradeThreshold = 1.2f;

        [ShowIf("IsEnabled")]
        [ToggleGroup("UseStarvationStrategy")]
        [LabelWidth(250)]
        [LabelText("Множитель веса при голодании")]
        [PropertyRange(0f, 1f)]
        public float StarvationWeightMultiplier = 0f;

        public override Type TargetActionType => typeof(FeedFishesAction);
    }

    [Serializable]
    [LabelText("Уборка трупов")]
    [Tooltip(
        "Как считается вес:\n• По умолчанию равен Базовому весу, если есть мертвые рыбы.\n• Стратегия Шлюза: Если аквариум заполнен, бот оставит трупы гнить (чтобы не дать рыбам размножаться), пока не накопит деньги на рыбу получше.")]
    public class CleanDeadFishActionConfig : BotActionConfig
    {
        [ShowIf("IsEnabled")]
        [ToggleGroup("UseAirlockStrategy", "Стратегия Шлюза (Трупы-блокаторы)")]
        public bool UseAirlockStrategy = false;

        [ShowIf("IsEnabled")]
        [ToggleGroup("UseAirlockStrategy")]
        [LabelWidth(250)]
        [LabelText("Жадность (множитель замены)")]
        public float UpgradeThreshold = 1.2f;

        [ShowIf("IsEnabled")]
        [ToggleGroup("UseAirlockStrategy")]
        [LabelWidth(250)]
        [LabelText("Множитель веса уборки")]
        [PropertyRange(0f, 1f)]
        public float AirlockWeightMultiplier = 0f;

        public override Type TargetActionType => typeof(CollectDeadFishAction);
    }

    [Serializable]
    [LabelText("Покупка рыб")]
    [Tooltip(
        "Как считается вес:\n• Выдает Базовый вес, если хватает денег на самую выгодную рыбу.\n• Если денег нет, выдает 0 (бот ждет).")]
    public class BuyFishActionConfig : BotActionConfig
    {
        public override Type TargetActionType => typeof(BuyFishAction);
    }

    [Serializable]
    [LabelText("Сбор монет")]
    [Tooltip(
        "Как работает:\n• Обычный сбор: бот смотрит на график (мало монет — игнорит, много — собирает).\n• Стратегия Копилки: если плавающих монет как раз не хватает на нужную покупку, бот умножает вес и срочно их забирает.")]
    public class CollectCoinsActionConfig : BotActionConfig
    {
        [Title("Обычный сбор (Кривая)")]
        [LabelWidth(250)]
        [LabelText("Предел монет (Ось X)")]
        [Tooltip("Сколько монет на экране берется за 1.0 (максимум) для графика ниже.")]
        [MinValue(1)]
        [ShowIf("IsEnabled")]
        public int MaxCoinsThreshold = 10;

        [LabelWidth(250)]
        [LabelText("Кривая желания")]
        [Tooltip("Ось X (от 0 до 1) - заполненность экрана монетами.\nОсь Y (от 0 до 1) - множитель базового веса.")]
        [ShowIf("IsEnabled")]
        public AnimationCurve CoinWeightCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        [ShowIf("IsEnabled")]
        [ToggleGroup("UseDesperationStrategy", "Стратегия Копилки (Жадность)")]
        public bool UseDesperationStrategy = false;

        [ShowIf("IsEnabled")]
        [ToggleGroup("UseDesperationStrategy")]
        [LabelWidth(250)]
        [LabelText("Множитель нужды")]
        [MinValue(1f)]
        public float DesperationMultiplier = 1.5f;

        public override Type TargetActionType => typeof(CollectCoinsAction);
    }

    [Serializable]
    [LabelText("Прокачка аквариума")]
    [Tooltip(
        "Как считается вес:\n• Вес равен 0, если аквариум свободен или не хватает денег.\n• После пробития Порога тесноты вес увеличивается до Базового веса.")]
    public class UpgradeAquariumActionConfig : BotActionConfig
    {
        [Title("Настройки расширения")]
        [LabelWidth(250)]
        [LabelText("Порог тесноты (в % / 100)")]
        [Tooltip("При какой заполненности аквариума бот начнет хотеть его расширить (0.8 = 80%).")]
        [PropertyRange(0f, 1f)]
        [ShowIf("IsEnabled")]
        public float MinCrowdedThreshold = 0.8f;

        public override Type TargetActionType => typeof(UpgradeAquariumAction);
    }
}