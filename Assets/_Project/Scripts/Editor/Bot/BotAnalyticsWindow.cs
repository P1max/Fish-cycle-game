using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Features.BotBalancer.Analytics;
using UnityEditor;
using UnityEngine;

namespace _Project.Scripts.Editor.Bot
{
    [InitializeOnLoad]
    public class BotAnalyticsWindow : EditorWindow
    {
        private SimulationReport _report;
        private string _lastLoadedPath;
        private Vector2 _scrollPos;

        private float _chartHeight = 250f;
        private float _zoomX = 1f;

        private const float PaddingLeft = 50f;
        private const float PaddingBottom = 25f;
        private const float PaddingTop = 10f;
        private const float PaddingRight = 15f;

        private readonly Color _colorCoins = new(1f, 0.84f, 0f);
        private readonly Color _colorCoinsEarned = new(0.6f, 0.8f, 0.2f);
        private readonly Color _colorFish = new(0f, 0.7f, 1f);
        private readonly Color _colorDead = new(1f, 0.3f, 0.3f);
        private readonly Color _colorBorn = new(0.2f, 1f, 0.4f);
        private readonly Color _colorBought = new(0.7f, 0.3f, 1f);
        private readonly Color _colorFeeder = new(1f, 0.5f, 0f);

        private GUIStyle _rightAlignedLabel;
        private GUIStyle _centerAlignedLabel;
        private GUIStyle _legendStyle;
        private GUIStyle _tooltipStyle;

        private class ChartLine
        {
            public string Name;
            public Color Color;
            public Func<IntervalSnapshot, float> Selector;
        }

        static BotAnalyticsWindow()
        {
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        private static void OnPlayModeChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredEditMode && EditorPrefs.HasKey("AutoOpenReportPath"))
            {
                var path = EditorPrefs.GetString("AutoOpenReportPath");

                EditorPrefs.DeleteKey("AutoOpenReportPath");
                EditorApplication.delayCall += () => OpenWithReport(path);
            }
        }

        [MenuItem("Tools/Аналитика симуляции")]
        public static void ShowWindow()
        {
            var window = GetWindow<BotAnalyticsWindow>("Аналитика симуляции");

            window.minSize = new Vector2(700, 600);
            window.Show();
        }

        public static void OpenWithReport(string path)
        {
            var window = GetWindow<BotAnalyticsWindow>("Analytics Dashboard");

            window._lastLoadedPath = path;
            window.LoadReport(path);
            window.Show();
        }

        private void OnEnable()
        {
            wantsMouseMove = true;
        }

        private void Update()
        {
            if (mouseOverWindow == this)
            {
                Repaint();
            }
        }

        private void InitStyles()
        {
            if (_tooltipStyle != null) return;

            _rightAlignedLabel = new GUIStyle(EditorStyles.miniLabel)
                { alignment = TextAnchor.MiddleRight, normal = { textColor = Color.white } };

            _centerAlignedLabel = new GUIStyle(EditorStyles.miniLabel)
                { alignment = TextAnchor.UpperCenter, normal = { textColor = Color.white } };

            _legendStyle = new GUIStyle(EditorStyles.miniBoldLabel);

            _tooltipStyle = new GUIStyle(EditorStyles.label)
            {
                alignment = TextAnchor.UpperLeft,
                richText = true,
                fontSize = 12
            };
        }

        private void OnGUI()
        {
            InitStyles();
            DrawHeader();

            if (_report == null || _report.Snapshots == null || _report.Snapshots.Count < 2)
            {
                EditorGUILayout.HelpBox("Загрузите файл отчета (.json или .csv), чтобы увидеть графики.", MessageType.Info);

                return;
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();

            _zoomX = EditorGUILayout.Slider("Ширина (Zoom X)", _zoomX, 1f, 20f);
            _chartHeight = EditorGUILayout.Slider("Высота (Zoom Y)", _chartHeight, 150f, 800f);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(10);

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            DrawChartSection("ЭКОНОМИКА (Монеты)", new List<ChartLine>
            {
                new() { Name = "Баланс игрока", Color = _colorCoins, Selector = s => s.CoinsBalance },
                new() { Name = "Всего заработано с рыб", Color = _colorCoinsEarned, Selector = s => s.CoinsCollectedAmount }
            });

            DrawChartSection("ПОПУЛЯЦИЯ АКВАРИУМА", new List<ChartLine>
            {
                new() { Name = "Живые рыбы", Color = _colorFish, Selector = s => s.FishesCount }
            });

            DrawChartSection("ДЕМОГРАФИЯ (Накопительно)", new List<ChartLine>
            {
                new() { Name = "Рождено рыб", Color = _colorBorn, Selector = s => s.BornCount },
                new() { Name = "Куплено рыб", Color = _colorBought, Selector = s => s.BoughtCount },
                new() { Name = "Умерло рыб", Color = _colorDead, Selector = s => s.DeadCount }
            });

            DrawChartSection("АКТИВНОСТЬ", new List<ChartLine>
            {
                new() { Name = "Использовано кормушек", Color = _colorFeeder, Selector = s => s.FeederUsedCount }
            });

            EditorGUILayout.Space(20);
            DrawStatsTable();
            EditorGUILayout.Space(20);

            EditorGUILayout.EndScrollView();
        }

        private void DrawHeader()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            if (GUILayout.Button("Загрузить JSON / CSV", EditorStyles.toolbarButton))
            {
                var path = EditorUtility.OpenFilePanelWithFilters("Выберите отчет симуляции", Application.dataPath,
                    new[] { "Отчеты симуляции", "json,csv", "All files", "*" });

                if (!string.IsNullOrEmpty(path)) LoadReport(path);
            }

            GUILayout.FlexibleSpace();

            if (!string.IsNullOrEmpty(_lastLoadedPath))
                GUILayout.Label($"Файл: {Path.GetFileName(_lastLoadedPath)}", EditorStyles.miniLabel);

            EditorGUILayout.EndHorizontal();
        }

        private void DrawChartSection(string title, List<ChartLine> lines)
        {
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);

            var baseWidth = position.width - 30f;
            var zoomedWidth = Mathf.Max(baseWidth, baseWidth * _zoomX);

            var containerRect = GUILayoutUtility.GetRect(zoomedWidth, _chartHeight);

            var plotRect = new Rect(
                containerRect.x + PaddingLeft,
                containerRect.y + PaddingTop,
                containerRect.width - PaddingLeft - PaddingRight,
                containerRect.height - PaddingTop - PaddingBottom
            );

            DrawPlotBackgroundAndGrid(plotRect, lines);
            DrawPlotLines(plotRect, lines);
            DrawLegend(containerRect, lines);
            DrawHoverTooltip(plotRect, lines);

            EditorGUILayout.Space(15);
        }

        private void DrawPlotBackgroundAndGrid(Rect plotRect, List<ChartLine> lines)
        {
            EditorGUI.DrawRect(plotRect, new Color(0.12f, 0.12f, 0.12f));

            var maxVal = GetMaxVal(lines);
            var maxTime = _report.Snapshots[^1].TimeSecond;

            Handles.color = new Color(0.3f, 0.3f, 0.3f, 0.5f);
            var ySteps = 4;

            for (var i = 0; i <= ySteps; i++)
            {
                var normalizedY = (float)i / ySteps;
                var y = plotRect.yMax - (normalizedY * plotRect.height);

                Handles.DrawLine(new Vector2(plotRect.x, y), new Vector2(plotRect.xMax, y));

                var val = normalizedY * maxVal;
                var labelRect = new Rect(plotRect.x - PaddingLeft, y - 8, PaddingLeft - 5, 16);
                GUI.Label(labelRect, FormatNumber(val), _rightAlignedLabel);
            }

            var baseSteps = 6;
            var xSteps = Mathf.RoundToInt(baseSteps * _zoomX);

            for (var i = 0; i <= xSteps; i++)
            {
                var normalizedX = (float)i / xSteps;
                var x = plotRect.x + (normalizedX * plotRect.width);

                Handles.DrawLine(new Vector2(x, plotRect.y), new Vector2(x, plotRect.yMax));

                var timeVal = normalizedX * maxTime;
                var labelRect = new Rect(x - 20, plotRect.yMax + 5, 40, 16);

                GUI.Label(labelRect, $"{timeVal:0}s", _centerAlignedLabel);
            }

            Handles.color = Color.white;
        }

        private void DrawPlotLines(Rect plotRect, List<ChartLine> lines)
        {
            var snapshots = _report.Snapshots;
            var maxVal = GetMaxVal(lines);
            var maxTime = snapshots[^1].TimeSecond;

            if (maxVal <= 0 || maxTime <= 0) return;

            foreach (var line in lines)
            {
                var points = new Vector3[snapshots.Count];

                for (var i = 0; i < snapshots.Count; i++)
                {
                    var x = plotRect.x + (snapshots[i].TimeSecond / maxTime) * plotRect.width;
                    var y = plotRect.yMax - (line.Selector(snapshots[i]) / maxVal) * plotRect.height;

                    points[i] = new Vector3(x, y, 0);
                }

                Handles.color = line.Color;
                Handles.DrawAAPolyLine(3f, points);
            }

            Handles.color = Color.white;
        }

        private void DrawHoverTooltip(Rect plotRect, List<ChartLine> lines)
        {
            var e = Event.current;
            var snapshots = _report.Snapshots;

            if (plotRect.Contains(e.mousePosition) && snapshots.Count > 0)
            {
                var maxTime = snapshots[^1].TimeSecond;
                var normalizedX = (e.mousePosition.x - plotRect.x) / plotRect.width;

                var index = Mathf.Clamp(Mathf.RoundToInt(normalizedX * (snapshots.Count - 1)), 0, snapshots.Count - 1);
                var closestSnapshot = snapshots[index];

                var exactX = plotRect.x + (closestSnapshot.TimeSecond / maxTime) * plotRect.width;

                Handles.color = new Color(1f, 1f, 1f, 0.4f);
                Handles.DrawLine(new Vector2(exactX, plotRect.y), new Vector2(exactX, plotRect.yMax));
                Handles.color = Color.white;

                var tooltipText = $"<color=white><b>Время: {closestSnapshot.TimeSecond}s</b></color>\n";

                foreach (var line in lines)
                {
                    var hexColor = ColorUtility.ToHtmlStringRGB(line.Color);
                    var value = line.Selector(closestSnapshot);

                    tooltipText += $"<color=#{hexColor}>■ {line.Name}:</color> <color=white>{value}</color>\n";
                }

                tooltipText = tooltipText.TrimEnd('\n');

                var content = new GUIContent(tooltipText);
                var size = _tooltipStyle.CalcSize(content);

                var tooltipX = exactX + 10;

                if (tooltipX + size.x > plotRect.xMax)
                {
                    tooltipX = exactX - size.x - 10;
                }

                var tooltipRect = new Rect(tooltipX, e.mousePosition.y, size.x + 10, size.y + 10);

                EditorGUI.DrawRect(tooltipRect, new Color(0.15f, 0.15f, 0.15f, 0.95f));

                Handles.color = new Color(0.4f, 0.4f, 0.4f, 1f);
                Handles.DrawWireCube(tooltipRect.center, tooltipRect.size);
                Handles.color = Color.white;

                GUI.Label(new Rect(tooltipX + 5, e.mousePosition.y + 5, size.x, size.y), content, _tooltipStyle);
            }
        }

        private void DrawLegend(Rect containerRect, List<ChartLine> lines)
        {
            var currentX = containerRect.x + PaddingLeft + 10;
            var y = containerRect.y + PaddingTop + 5;

            foreach (var line in lines)
            {
                _legendStyle.normal.textColor = line.Color;

                var content = new GUIContent($"■ {line.Name}");
                var size = _legendStyle.CalcSize(content);

                GUI.Label(new Rect(currentX, y, size.x, size.y), content, _legendStyle);
                currentX += size.x + 15;
            }
        }

        private float GetMaxVal(List<ChartLine> lines)
        {
            float max = 0;

            foreach (var line in lines)
            {
                var localMax = _report.Snapshots.Max(s => line.Selector(s));

                if (localMax > max) max = localMax;
            }

            return max > 0 ? max * 1.1f : 5f;
        }

        private string FormatNumber(float num)
        {
            if (num >= 1000) return (num / 1000f).ToString("0.0") + "k";

            return num.ToString("0");
        }

        private void DrawStatsTable()
        {
            var last = _report.Snapshots[^1];

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("ИТОГОВАЯ СВОДКА СЕССИИ", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField($"Всего куплено: {last.BoughtCount}");
            EditorGUILayout.LabelField($"Всего родилось: {last.BornCount}");
            EditorGUILayout.LabelField($"Всего умерло: {last.DeadCount}");
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField($"Использовано кормушек: {last.FeederUsedCount}");
            EditorGUILayout.LabelField($"Собрано монет с рыб: {last.CoinsCollectedAmount}");
            EditorGUILayout.LabelField($"Финальный баланс: {last.CoinsBalance}");
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        private void LoadReport(string path)
        {
            try
            {
                var extension = Path.GetExtension(path).ToLower();

                switch (extension)
                {
                    case ".json":
                    {
                        var json = File.ReadAllText(path);

                        _report = JsonUtility.FromJson<SimulationReport>(json);

                        break;
                    }
                    case ".csv":
                        _report = ParseCsv(path);

                        break;
                    default:
                        Debug.LogWarning("Неподдерживаемый формат: " + extension);

                        return;
                }

                _lastLoadedPath = path;
                Repaint();
            }
            catch (Exception e)
            {
                Debug.LogError($"Ошибка загрузки отчета: {e.Message}");
            }
        }

        private SimulationReport ParseCsv(string path)
        {
            var lines = File.ReadAllLines(path);

            if (lines.Length < 2) return new SimulationReport { Snapshots = new List<IntervalSnapshot>() };

            var separator = lines[0].Contains(';') ? ';' : ',';

            var headers = lines[0].Split(separator).Select(h => h.Trim().ToLower()).ToList();
            var snapshots = new List<IntervalSnapshot>();

            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i])) continue;

                var values = lines[i].Split(separator);

                var snapshot = new IntervalSnapshot
                {
                    TimeSecond = GetCsvValue(headers, values, "timesecond", "time"),
                    CoinsBalance = (int)GetCsvValue(headers, values, "coinsbalance", "balance"),
                    CoinsCollectedAmount = (int)GetCsvValue(headers, values, "coinscollectedamount", "collected"),
                    FishesCount = (int)GetCsvValue(headers, values, "fishescount", "fishes"),
                    BornCount = (int)GetCsvValue(headers, values, "borncount", "born"),
                    BoughtCount = (int)GetCsvValue(headers, values, "boughtcount", "bought"),
                    DeadCount = (int)GetCsvValue(headers, values, "deadcount", "dead"),
                    FeederUsedCount = (int)GetCsvValue(headers, values, "feederusedcount", "feeder")
                };

                snapshots.Add(snapshot);
            }

            return new SimulationReport { Snapshots = snapshots };
        }

        private float GetCsvValue(List<string> headers, string[] values, params string[] possibleKeys)
        {
            foreach (var key in possibleKeys)
            {
                var index = headers.IndexOf(key);

                if (index >= 0 && index < values.Length)
                {
                    var rawValue = values[index].Replace(',', '.');

                    if (float.TryParse(rawValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
                    {
                        return result;
                    }
                }
            }

            return 0f;
        }
    }
}