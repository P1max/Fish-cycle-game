using System.Collections.Generic;
using UnityEngine;

namespace Core.Configs.Providers
{
    public class LocalConfigProvider : GameConfigProvider
    {
        [SerializeField] private AquariumConfig _aquarium;
        [SerializeField] private FeederConfig _feeder;
        [SerializeField] private ConveyorConfig _conveyor;
        [SerializeField] private CommonFishConfig _commonFish;
        [SerializeField] private UpgradesConfig _upgrades;
        [SerializeField] private FishesDatabaseConfig _fishesDatabase;

        public override AquariumConfig GetAquariumConfig() => _aquarium;
        public override FeederConfig GetFeederConfig() => _feeder;
        public override ConveyorConfig GetConveyorConfig() => _conveyor;
        public override CommonFishConfig GetCommonFishConfig() => _commonFish;
        public override UpgradesConfig GetUpgradesConfig() => _upgrades;
        public override List<FishConfig> GetActiveFishes() => _fishesDatabase.Fishes;
    }
}