using Zenject;

namespace _Project.Core.States
{
    public class GameStateMachine
    {
        private readonly DiContainer _container;
        private IGameState _currentState;

        public GameStateMachine(DiContainer container)
        {
            _container = container;
        }

        public void Enter<TState>() where TState : IGameState
        {
            _currentState?.Exit();

            _currentState = _container.Resolve<TState>();

            _currentState.Enter();
        }
    }
}