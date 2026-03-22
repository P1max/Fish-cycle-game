using System.Collections.Generic;
using System.Linq;
using Core.Configs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class ConfigValidator
{
    private readonly GameConfigs _configs;

    public Dictionary<string, FishConfig> LoadedFishes { get; private set; }

    public ConfigValidator(GameConfigs configs)
    {
        _configs = configs;
    }

    public void LoadAndPatchFromJson()
    {
        LoadedFishes = Resources.LoadAll<FishConfig>("Configs/Fishes")
            .ToDictionary(fish => fish.Id, fish => fish);

        var jsonTextAsset = Resources.Load<TextAsset>("Configs/game_config");

        if (jsonTextAsset == null)
        {
            Debug.LogWarning("JSON не найден по пути Resources/Configs/game_config. Играем с базовыми SO.");

            return;
        }

        var jsonContent = jsonTextAsset.text;
        var json = JObject.Parse(jsonContent);

        if (json["aquarium"] != null)
        {
            JsonConvert.PopulateObject(json["aquarium"].ToString(), _configs.Aquarium);
            _configs.Aquarium.ValidateData();
        }

        if (json["feeder"] != null)
        {
            JsonConvert.PopulateObject(json["feeder"].ToString(), _configs.Feeder);
            _configs.Feeder.ValidateData();
        }
        
        if (json["boids"] != null)
        {
            JsonConvert.PopulateObject(json["boids"].ToString(), _configs.Boids);
            _configs.Boids.ValidateData();
        }

        if (json["fishConfigs"] is JArray fishArray)
        {
            foreach (var fishJson in fishArray)
            {
                var id = fishJson["id"]?.ToString();

                if (id != null && LoadedFishes.TryGetValue(id, out var targetFishSo))
                {
                    JsonConvert.PopulateObject(fishJson.ToString(), targetFishSo);
                    targetFishSo.ValidateData();
                }
                else Debug.LogWarning($"Рыбка с ID {id} есть в JSON, но SO не найден в Resources");
            }
        }
    }
}