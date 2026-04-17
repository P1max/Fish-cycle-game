using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Features.BotBalancer.AI.Configs;

namespace Features.BotBalancer.AI
{
    public class BotProfileConfig : ScriptableObject
    {
        [Title("Настройки мышления")]
        [LabelWidth(250)]
        [LabelText("Интервал принятия решений (сек)")]
        [MinValue(0.1f)]
        public float DecisionInterval = 1f;

        [Title("Модули поведения")]
        [SerializeReference]
        [TypeFilter("GetAvailableActionTypes")]
        public List<BotActionConfig> Actions = new();

        private IEnumerable<Type> GetAvailableActionTypes()
        {
            var allActionTypes = typeof(BotActionConfig).Assembly.GetTypes()
                .Where(t => !t.IsAbstract && typeof(BotActionConfig).IsAssignableFrom(t));

            foreach (var type in allActionTypes)
            {
                if (Actions == null || !Actions.Any(a => a != null && a.GetType() == type))
                    yield return type;
            }
        }
    }
}