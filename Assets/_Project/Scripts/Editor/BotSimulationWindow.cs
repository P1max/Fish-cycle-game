// Папка: Assets/_Project/Scripts/Editor/BotSimulationWindow.cs

using UnityEditor;
using UnityEngine;

namespace _Project.Editor
{
    public class BotSimulationWindow : EditorWindow
    {
        private float _timeScale = 11f;
        private int _simulationTimeSeconds = 60;
        private bool _disableUI;

        [MenuItem("Tools/Bot Balancer")]
        public static void ShowWindow()
        {
            GetWindow<BotSimulationWindow>("Bot Balancer");
        }

        private void OnGUI()
        {
            GUILayout.Label("Настройки симуляции", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            _timeScale = EditorGUILayout.Slider("Ускорение времени (TimeScale)", _timeScale, 1f, 100f);
            _simulationTimeSeconds = EditorGUILayout.IntSlider("Время симуляции (сек)", _simulationTimeSeconds, 10, 6000);
            _disableUI = EditorGUILayout.Toggle("Отключить UI (для FPS)", _disableUI);

            EditorGUILayout.Space();

            if (GUILayout.Button("Запустить симуляцию", GUILayout.Height(40)))
                RunSimulation();
        }

        private void RunSimulation()
        {
            if (EditorApplication.isPlaying)
            {
                Debug.LogWarning("Сначала остановите игру!");

                return;
            }

            EditorPrefs.SetBool("Bot_IsActive", true);
            EditorPrefs.SetFloat("Bot_TimeScale", _timeScale);
            EditorPrefs.SetInt("Bot_SimTime", _simulationTimeSeconds);
            EditorPrefs.SetBool("Bot_DisableUI", _disableUI);

            EditorApplication.isPlaying = true;
        }
    }
}