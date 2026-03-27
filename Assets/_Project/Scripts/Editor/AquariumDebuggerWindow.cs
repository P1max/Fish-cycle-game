using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace _Project.Scripts.Editor
{
    public class AquariumDebuggerWindow : OdinEditorWindow
    {
        [MenuItem("Tools/Aquarium Debugger")]
        private static void OpenWindow()
        {
            var window = GetWindow<AquariumDebuggerWindow>();

            window.titleContent = new GUIContent("Отладка Аквариума");
            window.Show();
        }

        [ShowInInspector]
        [ListDrawerSettings(IsReadOnly = true, ShowPaging = false, ShowFoldout = true, ListElementLabelName = "DisplayName")]
        [LabelText("Все рыбки в пуле (Живые и Мертвые)")]
        private List<FishDebugData> _allFishes = new();

        [Title("Мониторинг Рыбок", "Живое состояние аквариума", TitleAlignments.Centered)]
        [Button("Обновить список", ButtonSizes.Large), GUIColor(0.4f, 1f, 0.4f)]
        private void RefreshFishes()
        {
            _allFishes.Clear();

            var fishesInScene = FindObjectsByType<FishEntity>(FindObjectsSortMode.None);

            foreach (var fish in fishesInScene)
                _allFishes.Add(new FishDebugData(fish));
        }

        private void Update()
        {
            if (EditorApplication.isPlaying)
            {
                var fishesInScene = FindObjectsByType<FishEntity>(FindObjectsSortMode.None);

                if (_allFishes.Count != fishesInScene.Length)
                {
                    RefreshFishes();
                }

                Repaint();
            }
            else if (_allFishes.Count > 0)
            {
                _allFishes.Clear();
                Repaint();
            }
        }
    }

    [Serializable]
    public class FishDebugData
    {
        // Свойство используется одином
        public string DisplayName
        {
            get
            {
                if (FishObject == null) return "Удалена";

                var status = FishObject.IsAlive ? "Жива" : "Мертва";

                return $"{ConfigId} [{status}]";
            }
        }

        [TitleGroup("Ссылка на объект")]
        [HideLabel]
        public FishEntity FishObject;

        [InfoBox("Рыбка мертва. Ждёт удаления.", InfoMessageType.Error,
            "@FishObject != null && !FishObject.IsAlive")]
        [FoldoutGroup("Параметры (Заданы при создании)")]
        [ShowInInspector, LabelText("ID Базового Конфига")]
        public string ConfigId => FishObject != null && FishObject.Config != null ? FishObject.Config.Id : "N/A";

        [FoldoutGroup("Параметры (Заданы при создании)")]
        [ShowInInspector, LabelText("Базовая Скорость")]
        public float BaseSpeed => FishObject != null ? FishObject.BaseSpeed : 0f;

        [FoldoutGroup("Параметры (Заданы при создании)")]
        [ShowInInspector, LabelText("Время жизни (Старт)")]
        public float LifetimeStart => FishObject != null && FishObject.Config != null ? FishObject.Config.LifetimeSeconds : 0f;

        [FoldoutGroup("Текущее состояние", expanded: true)]
        [ShowInInspector, ProgressBar(0, 100, ColorGetter = "GetHungerColor")]
        [LabelText("Текущий голод")]
        public float CurrentHunger => FishObject != null && FishObject.Hunger != null ? FishObject.Hunger.CurrentHungerPercent : 0f;

        [FoldoutGroup("Текущее состояние")]
        [ShowInInspector, LabelText("Время до смерти")]
        [SuffixLabel("сек", overlay: true)]
        public float TimeToDeath => FishObject != null && FishObject.LifeCycle != null ? FishObject.LifeCycle.TimeToDeath : 0f;

        [FoldoutGroup("Текущее состояние")]
        [ShowInInspector, LabelText("Время до монетки")]
        [SuffixLabel("сек", overlay: true)]
        public float TimeToNextCoin => FishObject != null && FishObject.Economy != null ? FishObject.Economy.TimeToNextCoin : 0f;

        [FoldoutGroup("Текущее состояние")]
        [ShowInInspector, LabelText("Кулдаун размножения")]
        [SuffixLabel("сек", overlay: true)]
        public float TimeToBreed => FishObject != null && FishObject.Breeding != null ? FishObject.Breeding.TimeToBreed : 0f;

        public FishDebugData(FishEntity fish)
        {
            FishObject = fish;
        }

        private Color GetHungerColor()
        {
            return Color.Lerp(Color.green, Color.red, CurrentHunger / 100f);
        }
    }
}