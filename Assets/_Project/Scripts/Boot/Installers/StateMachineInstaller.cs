using _Project.Core.States;
using App;
using Zenject;

namespace Installers
{
    public class StateMachineInstaller : Installer<StateMachineInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<GameStateMachine>().AsSingle();
            Container.Bind<BootstrapState>().AsSingle();
            Container.Bind<PlayState>().AsSingle();

            Container.Bind<IInitializable>().To<GameBootstrapper>().AsSingle();
        }
    }
}