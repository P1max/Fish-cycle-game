// Папка: Assets/_Project/Scripts/Features/BotBalancer/BotSetupService.cs

using _Project.Core.Interfaces;
using UnityEngine;

namespace Features.BotBalancer
{
    public class BotSetupService : ICoreInit
    {
        public bool IsBotActive { get; private set; }
        public bool IsUIDisabled { get; private set; }

        public void Init()
        {
#if UNITY_EDITOR
            IsBotActive = UnityEditor.EditorPrefs.GetBool("Bot_IsActive", false);

            if (IsBotActive)
            {
                var timeScale = UnityEditor.EditorPrefs.GetFloat("Bot_TimeScale", 1f);

                IsUIDisabled = UnityEditor.EditorPrefs.GetBool("Bot_DisableUI", true);

                Time.timeScale = timeScale;
                Time.fixedDeltaTime = Mathf.Clamp(0.02f * timeScale, 0.02f, 0.25f);

                Debug.Log($"Игра будет запущена в режиме симуляции.");

                UnityEditor.EditorPrefs.SetBool("Bot_IsActive", false);
            }
            else
            {
                Time.timeScale = 1f;
                Time.fixedDeltaTime = 0.02f;
            }
#else
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;
            IsBotActive = false;
#endif
        }
    }
}