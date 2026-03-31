#if UNITY_EDITOR
using System.IO;
using System.Linq;
using Core.Configs;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace _Project.Scripts.Editor
{
    public static class JsonConfigExporter
    {
        [MenuItem("Tools/Настройки Игры/Экспортировать JSON шаблона", priority = 100)]
        public static void ExportConfigsToJson()
        {
            var aquarium = GetSingleAsset<AquariumConfig>();
            var feeder = GetSingleAsset<FeederConfig>();
            var boids = GetSingleAsset<CommonFishConfig>();
            var store = GetSingleAsset<ConveyorConfig>();
            var fishDb = GetSingleAsset<FishesDatabaseConfig>();

            if (aquarium == null || feeder == null || boids == null || store == null || fishDb == null)
            {
                EditorUtility.DisplayDialog("Ошибка экспорта", "Один или несколько конфигов не найдены в проекте! Проверьте консоль.",
                    "OK");

                return;
            }

            var path = EditorUtility.SaveFilePanel("Сохранить настройки", "Assets", "game_config_template", "json");

            if (string.IsNullOrEmpty(path))
                return;

            var exportData = new
            {
                aquarium = new
                {
                    startCoins = aquarium.StartCoins,
                    startFishCount = aquarium.StartFishCount,
                    maxFishCount = aquarium.MaxFishCount
                },
                feeder = new
                {
                    cooldownSeconds = feeder.CooldownSeconds,
                    totalHungerRestorePerUse = feeder.TotalHungerRestorePerUse,
                    foodPiecesCount = new { x = (float)feeder.FoodPiecesCount.x, y = (float)feeder.FoodPiecesCount.y }
                },
                boids = new
                {
                    marginXSides = boids.MarginXSides,
                    marginTop = boids.MarginTop,
                    marginBottom = boids.MarginBottom,
                    boundsWeight = boids.BoundsWeight,
                    neighborRadius = boids.NeighborRadius,
                    separationRadius = boids.SeparationRadius,
                    alignmentWeight = boids.AlignmentWeight,
                    cohesionWeight = boids.CohesionWeight,
                    separationWeight = boids.SeparationWeight,
                    wanderWeight = boids.WanderWeight,
                    foodWeight = boids.FoodWeight,
                    foodSearchRadius = boids.FoodSearchRadius
                },
                store = new
                {
                    conveyerSpeed = store.ConveyorSpeed,
                    freeFishQuality = new { x = store.FreeQualityRange.x, y = store.FreeQualityRange.y },
                    coinsForFreeFish = store.CoinsForFreeFish,
                    fishesCountForFreeFish = store.FishesCountForFreeFish
                },
                fishConfigs = fishDb.Fishes.Select(f => new
                {
                    id = f.Id,
                    type = f.Type,
                    price = f.Price,
                    canBeFree = f.CanBeFree,
                    sizeModifier = new { x = f.SizeModifier.x, y = f.SizeModifier.y },
                    lifetimeSeconds = f.LifetimeSeconds,
                    hungerGrowthPercentPerSecond = f.HungerGrowthPercentPerSecond,
                    breedChancePercent = f.BreedChancePercent,
                    breedCooldownSeconds = f.BreedCooldownSeconds,
                    incomeCoins = f.IncomeCoins,
                    incomeCooldownSeconds = f.IncomeCooldownSeconds,
                    normalSpeedRange = new { x = f.NormalSpeedRange.x, y = f.NormalSpeedRange.y },
                    steerSpeed = f.SteerSpeed,
                    maxTiltAngle = f.MaxTiltAngle
                }).ToList()
            };

            var jsonResult = JsonConvert.SerializeObject(exportData, Formatting.Indented);

            File.WriteAllText(path, jsonResult);

            AssetDatabase.Refresh();

            Debug.Log($"<color=green>Успех!</color> Конфиги успешно экспортированы в JSON по пути: {path}");
            EditorUtility.DisplayDialog("Успех", "JSON файл успешно создан!", "Отлично");
        }

        private static T GetSingleAsset<T>() where T : ScriptableObject
        {
            var guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);

            if (guids.Length > 0)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);

                return AssetDatabase.LoadAssetAtPath<T>(assetPath);
            }

            Debug.LogWarning($"Конфиг {typeof(T).Name} не найден в проекте!");

            return null;
        }
    }
}
#endif