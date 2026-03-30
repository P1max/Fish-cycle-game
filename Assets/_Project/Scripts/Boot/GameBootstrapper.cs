using _Project.Core.States;
using Zenject;

namespace App
{
    public class GameBootstrapper : IInitializable
    {
        private readonly GameStateMachine _stateMachine;

        public GameBootstrapper(GameStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void Initialize()
        {
            _stateMachine.Enter<BootstrapState>();
        }
    }
}