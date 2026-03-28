// ReSharper disable RedundantUsingDirective

using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
#endif

namespace Core.Configs
{
    public class GameConfigs : ScriptableObject
    {
        [TitleGroup("Режим работы (Источник данных)", Alignment = TitleAlignments.Centered)]
        [InfoBox("Если включено — при старте игры данные будут перезаписаны из JSON в папке Resources.", InfoMessageType.Warning,
            "UseJsonConfig")]
        [InfoBox("Если выключено — игра использует значения из этих ScriptableObject напрямую.", InfoMessageType.Info, "@!UseJsonConfig")]
        [GUIColor(0.5f, 1f, 0.5f)]
        [ToggleLeft]
        public bool UseJsonConfig;

        [Space(10)]
        [TitleGroup("Системные конфиги", Alignment = TitleAlignments.Centered)]
        [InlineEditor(InlineEditorObjectFieldModes.Boxed)]
        public AquariumConfig Aquarium;

        [TitleGroup("Системные конфиги")]
        [InlineEditor(InlineEditorObjectFieldModes.Boxed)]
        public FeederConfig Feeder;

        [TitleGroup("Системные конфиги")]
        [InlineEditor(InlineEditorObjectFieldModes.Boxed)]
        public CommonFishConfig CommonFish;

        [TitleGroup("Системные конфиги")]
        [InlineEditor(InlineEditorObjectFieldModes.Boxed)]
        public ConveyorConfig Conveyor;

        [TitleGroup("Системные конфиги")]
        [InlineEditor(InlineEditorObjectFieldModes.Boxed)]
        public FishesDatabaseConfig FishesDatabase;
        
        [TitleGroup("Системные конфиги")]
        [InlineEditor(InlineEditorObjectFieldModes.Boxed)]
        public UpgradesConfig UpgradesConfig;

#if UNITY_EDITOR
        [Space(10)]
        [TitleGroup("Инструменты разработчика", "Действия выполняются только в редакторе!", Alignment = TitleAlignments.Centered)]
        [LabelText("JSON файл")]
        [InfoBox("Положи сюда JSON-файл, чтобы кнопка стала активной.", InfoMessageType.Warning, "@JsonFile == null")]
        public TextAsset JsonFile;

        [TitleGroup("Инструменты разработчика")]
        [Button("Запечь внешний JSON в SO", ButtonSizes.Large), GUIColor(0.2f, 0.7f, 1f)]
        [PropertyTooltip("Перезаписать исходные ScriptableObject данными из указанного выше файла.")]
        [EnableIf("@JsonFile != null")]
        private void BakeJsonIntoScriptableObjects()
        {
            if (JsonFile == null)
            {
                Debug.LogError("Файл JSON не назначен!");

                return;
            }

            var jsonContent = JsonFile.text;
            var json = JObject.Parse(jsonContent);

            if (json["aquarium"] != null && Aquarium != null)
            {
                JsonConvert.PopulateObject(json["aquarium"].ToString(), Aquarium);
                Aquarium.ValidateData();
                EditorUtility.SetDirty(Aquarium);
            }

            if (json["feeder"] != null && Feeder != null)
            {
                JsonConvert.PopulateObject(json["feeder"].ToString(), Feeder);
                Feeder.ValidateData();
                EditorUtility.SetDirty(Feeder);
            }

            if (json["store"] != null && Conveyor != null)
            {
                JsonConvert.PopulateObject(json["store"].ToString(), Conveyor);
                EditorUtility.SetDirty(Conveyor);
            }

            if (json["boids"] != null && CommonFish != null)
            {
                JsonConvert.PopulateObject(json["boids"].ToString(), CommonFish);
                CommonFish.ValidateData();
                EditorUtility.SetDirty(CommonFish);
            }

            if (json["fishConfigs"] is JArray fishArray && FishesDatabase != null)
            {
                foreach (var fishJson in fishArray)
                {
                    var id = fishJson["id"]?.ToString();

                    if (string.IsNullOrEmpty(id)) continue;

                    var targetFish = FishesDatabase.Fishes.FirstOrDefault(f => f.Id == id);

                    if (targetFish != null)
                    {
                        JsonConvert.PopulateObject(fishJson.ToString(), targetFish);
                        targetFish.ValidateData();
                    }
                    else
                    {
                        Debug.LogWarning(
                            $"[GameConfigs] Рыбка {id} есть в JSON, но ее нет в FishesDatabase! Создайте пустой элемент с таким ID.");
                    }
                }

                EditorUtility.SetDirty(FishesDatabase);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("<color=cyan>[GameConfigs]</color> Конфиги успешно заменены из файла " + JsonFile.name + "!");
        }
#endif
    }
}