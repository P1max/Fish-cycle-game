using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Core.Configs
{
    public class FishesDatabaseConfig : ScriptableObject
    {
        [Title("База всех рыб")]
        [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "Id")]
        [Searchable] // Поиск рыб по ID в инспекторе
        public List<FishConfig> Fishes = new();
        
        private void OnValidate()
        {
            if (Fishes == null) return;
            
            foreach (var fish in Fishes)
            {
                fish?.ValidateData();
            }
        }
    }
}