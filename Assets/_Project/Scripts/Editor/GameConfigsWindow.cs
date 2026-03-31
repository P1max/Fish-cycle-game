#if UNITY_EDITOR
using Core.Configs;
using Core.Configs.Providers;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace _Project.Scripts.Editor
{
    public class GameConfigsWindow : OdinMenuEditorWindow
    {
        [MenuItem("Tools/Настройки Игры/Открыть окно", priority = 1)]
        private static void OpenWindow()
        {
            var window = GetWindow<GameConfigsWindow>();

            window.titleContent = new GUIContent("Настройки Игры");
            window.Show();
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree(supportsMultiSelect: false)
            {
                Config = { DrawSearchToolbar = true }
            };

            tree.Add("Источник данных", GetSingleAsset<DataSource>(), SdfIconType.Server);

            var providers = tree.AddAllAssetsAtPath("Источник данных/Провайдеры", "Assets", typeof(GameConfigProvider), true, true);

            foreach (var item in providers) item.SdfIcon = SdfIconType.Hdd;

            tree.Add("Аквариум/Базовый конфиг", GetSingleAsset<AquariumConfig>(), SdfIconType.House);
            tree.Add("Аквариум/Прокачка (Уровни)", GetSingleAsset<UpgradesConfig>(), SdfIconType.GraphUp);
            tree.Add("Аквариум/Кормушка", GetSingleAsset<FeederConfig>(), SdfIconType.Droplet);
            tree.Add("Аквариум/Лента (Conveyor)", GetSingleAsset<ConveyorConfig>(), SdfIconType.BoxSeam);

            tree.Add("Рыбы/База рыб (Fishes)", GetSingleAsset<FishesDatabaseConfig>(), SdfIconType.ListUl);
            tree.Add("Рыбы/Настройки", GetSingleAsset<CommonFishConfig>(), SdfIconType.ExclamationTriangle);

            return tree;
        }

        protected override void OnBeginDrawEditors()
        {
            var selected = MenuTree.Selection;

            if (selected.Count > 0)
            {
                SirenixEditorGUI.BeginHorizontalToolbar();
                GUILayout.Label(selected[0].Name, new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
                SirenixEditorGUI.EndHorizontalToolbar();

                if (selected[0].Value == null)
                {
                    GUILayout.Space(20);

                    EditorGUILayout.HelpBox(
                        $"Вы выбрали категорию «{selected[0].Name}».\n\n" +
                        "Пожалуйста, раскройте этот пункт в меню слева и выберите конкретный конфиг для настройки.",
                        MessageType.Info
                    );

                    if (Event.current.type == EventType.Layout && selected[0].ChildMenuItems.Count > 0)
                    {
                        selected[0].Toggled = true;
                    }
                }
            }
        }

        private T GetSingleAsset<T>() where T : ScriptableObject
        {
            string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);

            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);

                return AssetDatabase.LoadAssetAtPath<T>(path);
            }

            Debug.LogWarning($"Конфиг {typeof(T).Name} не найден в проекте! Создай его через Create Asset Menu.");

            return null;
        }
    }
}
#endif