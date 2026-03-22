using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.Loaders
{
    public class FishesLoader
    {
        public Dictionary<string, FishConfig> LoadedFishesDict { get; private set; }
        
        public List<FishConfig> LoadedFishesList { get; private set; }
        

        public FishesLoader()
        {
            LoadedFishesDict = Resources.LoadAll<FishConfig>("Configs/Fishes")
                .ToDictionary(fish => fish.Id, fish => fish);
            
            LoadedFishesList = LoadedFishesDict.Values.ToList();
        }
    }
}