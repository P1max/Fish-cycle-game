using Features.BotBalancer.AI;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace _Project.Scripts.Editor
{
    public class BotSimulationWindow : OdinEditorWindow
    {
        [MenuItem("Tools/Бот для симуляции")]
        public static void ShowWindow()
        {
            GetWindow<BotSimulationWindow>("Бот для симуляции");
        }

        [Title("Настройки симуляции", "Параметры для запуска бота", TitleAlignments.Centered)]
        [LabelText("Ускорение времени (TimeScale)")]
        [PropertyRange(1f, 100f)]
        public float TimeScale = 11f;

        [LabelText("Время симуляции (сек игрового времени)")]
        [LabelWidth(260)]
        [PropertyRange(10, 60000)]
        public int SimulationTimeSeconds = 60;

        [LabelText("Интервал для создания одного репорта")]
        [LabelWidth(260)]
        [PropertyRange(0.1f, 10)]
        public float ReportsInterval = 1f;

        [LabelText("Отключить UI (для FPS)")]
        public bool DisableUI;

        [Title("Профиль бота", "Настройки поведения бота во время симуляции", TitleAlignments.Centered)]
        [LabelText("Текущий профиль")]
        [Required("Пожалуйста, выберите или создайте профиль бота (ScriptableObject)")]
        [InlineEditor(InlineEditorObjectFieldModes.Boxed)]
        public BotProfileConfig CurrentProfile;

        [Button("Создать новый профиль", ButtonSizes.Medium)]
        [GUIColor(0.6f, 1f, 0.6f)]
        [ShowIf("@CurrentProfile == null")]
        private void CreateNewProfile()
        {
            var newProfile = CreateInstance<BotProfileConfig>();

            var path = EditorUtility.SaveFilePanelInProject(
                "Сохранить профиль",
                "NewBotProfile",
                "asset",
                "Выберите папку для сохранения профиля");

            if (!string.IsNullOrEmpty(path))
            {
                AssetDatabase.CreateAsset(newProfile, path);
                AssetDatabase.SaveAssets();
                
                CurrentProfile = newProfile;
            }
        }

        [Button("Запустить симуляцию", ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1f)]
        [EnableIf("@CurrentProfile != null")]
        private void RunSimulation()
        {
            if (EditorApplication.isPlaying)
            {
                Debug.LogWarning("Сначала остановите игру!");

                return;
            }

            EditorPrefs.SetBool("Bot_IsActive", true);
            EditorPrefs.SetFloat("Bot_TimeScale", TimeScale);
            EditorPrefs.SetInt("Bot_SimTime", SimulationTimeSeconds);
            EditorPrefs.SetBool("Bot_DisableUI", DisableUI);
            EditorPrefs.SetFloat("Bot_ReportsInterval", ReportsInterval);

            var profilePath = AssetDatabase.GetAssetPath(CurrentProfile);
            
            EditorPrefs.SetString("Bot_ProfilePath", profilePath);

            EditorApplication.isPlaying = true;
        }
    }
}