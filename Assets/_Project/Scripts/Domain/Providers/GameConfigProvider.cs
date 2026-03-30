using System.Collections.Generic;
using UnityEngine;

namespace Core.Configs.Providers
{
    public abstract class GameConfigProvider : ScriptableObject
    {
        public abstract AquariumConfig GetAquariumConfig();
        public abstract FeederConfig GetFeederConfig();
        public abstract ConveyorConfig GetConveyorConfig();
        public abstract CommonFishConfig GetCommonFishConfig();
        public abstract UpgradesConfig GetUpgradesConfig();
        public abstract List<FishConfig> GetActiveFishes();
    }
}