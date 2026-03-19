using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Configs
{
    public class GameConfigs : ScriptableObject
    {
        [Title("Системные конфиги")]
        public AquariumConfig Aquarium;
        public FeederConfig Feeder;
    }
}