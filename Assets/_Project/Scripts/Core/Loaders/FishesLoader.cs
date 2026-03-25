using System.Collections.Generic;
using System.Linq;

namespace Core.Loaders
{
    public class FishesLoader
    {
        public Dictionary<string, FishConfig> LoadedFishesDict { get; private set; }

        public FishesLoader(List<FishConfig> fishesList)
        {
            LoadedFishesDict = fishesList.ToDictionary(fish => fish.Id, fish => fish);
        }
    }
}