using Core.Configs;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class GameplaySceneInstaller : MonoInstaller
    {
        [SerializeField] private DataSource _dataSource;

        public override void InstallBindings()
        {
            ConfigsInstaller.Install(Container, _dataSource);
            PoolsInstaller.Install(Container);
            GameplayCoreInstaller.Install(Container);
            BotInstaller.Install(Container);
            UIInstaller.Install(Container);
            StateMachineInstaller.Install(Container);
        }
    }
}