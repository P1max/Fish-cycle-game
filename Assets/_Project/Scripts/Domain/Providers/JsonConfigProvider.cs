using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Core.Configs.Providers
{
    public class JsonConfigProvider : GameConfigProvider
    {
        [TitleGroup("Источник JSON")]
        [SerializeField] private TextAsset _jsonFile;

        [TitleGroup("Базовые SO (Для клонирования)")]
        [SerializeField] private AquariumConfig _baseAquarium;
        [SerializeField] private FeederConfig _baseFeeder;
        [SerializeField] private ConveyorConfig _baseConveyor;
        [SerializeField] private CommonFishConfig _baseCommonFish;
        [SerializeField] private UpgradesConfig _baseUpgrades;
        [SerializeField] private FishesDatabaseConfig _baseFishesDatabase;

        private AquariumConfig _aquarium;
        private FeederConfig _feeder;
        private ConveyorConfig _conveyor;
        private CommonFishConfig _commonFish;
        private UpgradesConfig _upgrades;
        private List<FishConfig> _fishes;
        private bool _isInitialized;

        private void OnEnable() => _isInitialized = false;

        private void Initialize()
        {
            if (_isInitialized) return;

            _aquarium = Instantiate(_baseAquarium);
            _feeder = Instantiate(_baseFeeder);
            _conveyor = Instantiate(_baseConveyor);
            _commonFish = Instantiate(_baseCommonFish);
            _upgrades = Instantiate(_baseUpgrades);
            _fishes = _baseFishesDatabase.Fishes.Select(f => f.Clone()).ToList();

            if (_jsonFile != null)
            {
                var json = JObject.Parse(_jsonFile.text);

                if (json["aquarium"] != null)
                {
                    JsonConvert.PopulateObject(json["aquarium"].ToString(), _aquarium);
                    _aquarium.ValidateData();
                }

                if (json["feeder"] != null)
                {
                    JsonConvert.PopulateObject(json["feeder"].ToString(), _feeder);
                    _feeder.ValidateData();
                }

                if (json["store"] != null)
                {
                    JsonConvert.PopulateObject(json["store"].ToString(), _conveyor);
                    _conveyor.ValidateData();
                }

                if (json["boids"] != null)
                {
                    JsonConvert.PopulateObject(json["boids"].ToString(), _commonFish);
                    _commonFish.ValidateData();
                }

                if (json["fishConfigs"] is JArray fishArray)
                {
                    foreach (var fishJson in fishArray)
                    {
                        var id = fishJson["id"]?.ToString();
                        var targetFish = _fishes.FirstOrDefault(f => f.Id == id);

                        if (targetFish != null)
                        {
                            JsonConvert.PopulateObject(fishJson.ToString(), targetFish);
                            targetFish.ValidateData();
                        }
                    }
                }
            }

            _isInitialized = true;
        }

        public override AquariumConfig GetAquariumConfig()
        {
            Initialize();

            return _aquarium;
        }

        public override FeederConfig GetFeederConfig()
        {
            Initialize();

            return _feeder;
        }

        public override ConveyorConfig GetConveyorConfig()
        {
            Initialize();

            return _conveyor;
        }

        public override CommonFishConfig GetCommonFishConfig()
        {
            Initialize();

            return _commonFish;
        }

        public override UpgradesConfig GetUpgradesConfig()
        {
            Initialize();

            return _upgrades;
        }

        public override List<FishConfig> GetActiveFishes()
        {
            Initialize();

            return _fishes;
        }

#if UNITY_EDITOR
        [TitleGroup("Инструменты разработчика")]
        [Button("Запечь внешний JSON в базовые SO", ButtonSizes.Large), GUIColor(0.2f, 0.7f, 1f)]
        private void BakeJsonIntoScriptableObjects()
        {
            if (_jsonFile == null) return;

            var json = JObject.Parse(_jsonFile.text);

            if (json["aquarium"] != null)
            {
                JsonConvert.PopulateObject(json["aquarium"].ToString(), _baseAquarium);
                _baseAquarium.ValidateData();
                EditorUtility.SetDirty(_baseAquarium);
            }

            if (json["feeder"] != null)
            {
                JsonConvert.PopulateObject(json["feeder"].ToString(), _baseFeeder);
                _baseFeeder.ValidateData();
                EditorUtility.SetDirty(_baseFeeder);
            }

            if (json["store"] != null)
            {
                JsonConvert.PopulateObject(json["store"].ToString(), _baseConveyor);
                EditorUtility.SetDirty(_baseConveyor);
            }

            if (json["boids"] != null)
            {
                JsonConvert.PopulateObject(json["boids"].ToString(), _baseCommonFish);
                _baseCommonFish.ValidateData();
                EditorUtility.SetDirty(_baseCommonFish);
            }

            if (json["fishConfigs"] is JArray fishArray)
            {
                foreach (var fishJson in fishArray)
                {
                    var id = fishJson["id"]?.ToString();
                    var targetFish = _baseFishesDatabase.Fishes.FirstOrDefault(f => f.Id == id);

                    if (targetFish != null)
                    {
                        JsonConvert.PopulateObject(fishJson.ToString(), targetFish);
                        targetFish.ValidateData();
                    }
                }

                EditorUtility.SetDirty(_baseFishesDatabase);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Запекание завершено!");
        }
#endif
    }
}