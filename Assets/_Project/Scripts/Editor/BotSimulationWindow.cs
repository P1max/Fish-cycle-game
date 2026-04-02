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
        [PropertyRange(1f, 300f)]
        public float TimeScale = 11f;

        [LabelText("Время симуляции (сек игрового времени)")]
        [LabelWidth(260)]
        [PropertyRange(10, 6000)]
        public int SimulationTimeSeconds = 60;

        [LabelText("Интервал для создания одного репорта")]
        [LabelWidth(260)]
        [PropertyRange(0.1f, 10)]
        public float ReportsInterval = 1f;

        [LabelText("Отключить UI (для FPS)")]
        public bool DisableUI;

        [Button("Запустить симуляцию", ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1f)]
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

            EditorApplication.isPlaying = true;
        }
    }
}