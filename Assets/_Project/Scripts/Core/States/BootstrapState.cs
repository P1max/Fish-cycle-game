using System.Collections.Generic;
using _Project.Core.Interfaces;
using DG.Tweening;
using UnityEngine;

namespace _Project.Core.States
{
    public class BootstrapState : IGameState
    {
        private readonly GameStateMachine _stateMachine;
        private readonly List<IGameInit> _initables;

        public BootstrapState(GameStateMachine stateMachine, List<IGameInit> initables)
        {
            _stateMachine = stateMachine;
            _initables = initables;
        }

        public void Enter()
        {
            Debug.Log("Инициализация систем");

            DOTween.SetTweensCapacity(500, 250);

            foreach (var initable in _initables)
            {
                initable.Init();
            }

            _stateMachine.Enter<PlayState>();
        }

        public void Exit()
        {
        }
    }
}