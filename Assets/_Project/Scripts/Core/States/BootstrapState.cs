using System.Collections.Generic;
using _Project.Core.Interfaces;
using DG.Tweening;
using Features.BotBalancer;
using UI.Core;
using UnityEngine;

namespace _Project.Core.States
{
    public class BootstrapState : IGameState
    {
        private readonly GameStateMachine _stateMachine;
        private readonly List<ICoreInit> _coreInits;
        private readonly List<IGameplayInit> _gameplayInits;
        private readonly List<IUIInit> _uiInits;
        private readonly BotSetupService _botSetup;
        private readonly UIRoot _uiRoot;

        public BootstrapState(
            GameStateMachine stateMachine,
            List<ICoreInit> coreInits,
            List<IGameplayInit> gameplayInits,
            List<IUIInit> uiInits,
            BotSetupService botSetup,
            UIRoot uiRoot)
        {
            _stateMachine = stateMachine;
            _coreInits = coreInits;
            _gameplayInits = gameplayInits;
            _uiInits = uiInits;
            _botSetup = botSetup;
            _uiRoot = uiRoot;
        }

        private void InitGroup(IEnumerable<IInit> group)
        {
            foreach (var initable in group)
                initable.Init();
        }

        public void Enter()
        {
            DOTween.SetTweensCapacity(800, 350);

            InitGroup(_coreInits);
            InitGroup(_gameplayInits);

            if (!_botSetup.IsUIDisabled)
                InitGroup(_uiInits);
            else
            {
                _uiRoot.SetActive(false);
                
                Debug.Log("Слой UI отключен настройками бота.");
            }

            _stateMachine.Enter<PlayState>();
        }

        public void Exit()
        {
        }
    }
}