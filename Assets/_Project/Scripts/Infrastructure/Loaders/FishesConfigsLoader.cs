using System.Collections.Generic;
using System.Linq;

namespace Core.Loaders
{
    public class FishesConfigsLoader
    {
        public Dictionary<string, FishConfig> LoadedFishesDict { get; private set; }

        public FishesConfigsLoader(List<FishConfig> fishesList)
        {
            LoadedFishesDict = fishesList.ToDictionary(fish => fish.Id, fish => fish);
        }
    }
}