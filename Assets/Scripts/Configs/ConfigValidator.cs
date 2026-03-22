using Core.Configs;
using Core.Loaders;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class ConfigValidator
{
    public ConfigValidator(GameConfigs configs, FishesLoader fishesLoader)
    {
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
            JsonConvert.PopulateObject(json["aquarium"].ToString(), configs.Aquarium);
            configs.Aquarium.ValidateData();
        }

        if (json["feeder"] != null)
        {
            JsonConvert.PopulateObject(json["feeder"].ToString(), configs.Feeder);
            configs.Feeder.ValidateData();
        }
        
        if (json["boids"] != null)
        {
            JsonConvert.PopulateObject(json["boids"].ToString(), configs.CommonFish);
            configs.CommonFish.ValidateData();
        }

        if (json["fishConfigs"] is JArray fishArray)
        {
            foreach (var fishJson in fishArray)
            {
                var id = fishJson["id"]?.ToString();

                if (id != null && fishesLoader.LoadedFishesDict.TryGetValue(id, out var targetFishSo))
                {
                    JsonConvert.PopulateObject(fishJson.ToString(), targetFishSo);
                    targetFishSo.ValidateData();
                }
                else Debug.LogWarning($"Рыбка с ID {id} есть в JSON, но SO не найден в Resources");
            }
        }
        
        Debug.Log("Конфиги свалидированы");
    }
}