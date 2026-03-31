using System.Collections.Generic;
using _Project.Core.Interfaces;
using DG.Tweening;
using UnityEngine;

namespace _Project.Core.States
{
    public class BootstrapState : IGameState
    {
        private readonly GameStateMachine _stateMachine;
        private readonly List<ICoreInit> _coreInits;
        private readonly List<IGameplayInit> _gameplayInits;
        private readonly List<IUIInit> _uiInits;

        public BootstrapState(
            GameStateMachine stateMachine,
            List<ICoreInit> coreInits,
            List<IGameplayInit> gameplayInits,
            List<IUIInit> uiInits)
        {
            _stateMachine = stateMachine;
            _coreInits = coreInits;
            _gameplayInits = gameplayInits;
            _uiInits = uiInits;
        }

        private void InitGroup(IEnumerable<IInit> group, string groupName)
        {
            foreach (var initable in group)
                initable.Init();

            Debug.Log($"[Bootstrap] Слой {groupName} инициализирован");
        }

        public void Enter()
        {
            Debug.Log("Инициализация систем");

            DOTween.SetTweensCapacity(500, 250);

            InitGroup(_coreInits, "Core");
            InitGroup(_gameplayInits, "Gameplay");
            InitGroup(_uiInits, "UI");

            _stateMachine.Enter<PlayState>();
        }

        public void Exit()
        {
        }
    }
}