using System.Linq;
using Features.BotBalancer;
using Features.BotBalancer.AI;
using Features.BotBalancer.Analytics;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class BotInstaller : Installer<BotInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<BotSetupService>().AsSingle();

            var isBotActive = false;

#if UNITY_EDITOR
            isBotActive = UnityEditor.EditorPrefs.GetBool("Bot_IsActive", false);
#endif

            if (!isBotActive) return;

            BotProfileConfig activeProfile = null;

#if UNITY_EDITOR
            var profilePath = UnityEditor.EditorPrefs.GetString("Bot_ProfilePath", "");

            if (!string.IsNullOrEmpty(profilePath))
                activeProfile = UnityEditor.AssetDatabase.LoadAssetAtPath<BotProfileConfig>(profilePath);
#endif

            if (activeProfile)
            {
                Debug.Log($"Для симуляции загружен профиль: {activeProfile.name}");
            }
            else
            {
                Debug.LogWarning("Профиль из окна симуляции не найден. Бот не будет запущен.");

                return;
            }

            Container.Bind<BotProfileConfig>().FromInstance(activeProfile).AsSingle();

            foreach (var actionConfig in activeProfile.Actions.Where(actionConfig => actionConfig.IsEnabled))
            {
                Container.Bind(actionConfig.GetType()).FromInstance(actionConfig).AsSingle();
                Container.Bind(typeof(IBotAction)).To(actionConfig.TargetActionType).AsSingle();
            }

            Container.BindInterfacesAndSelfTo<BotController>().AsSingle();
            Container.BindInterfacesAndSelfTo<BotAnalyticsService>().AsSingle();
        }
    }
}