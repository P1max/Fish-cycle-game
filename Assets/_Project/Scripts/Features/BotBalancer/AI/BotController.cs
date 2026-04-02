using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Features.BotBalancer.AI
{
    public class BotController : ITickable
    {
        private readonly List<IBotAction> _availableActions;
        private readonly BotProfileConfig _profile;
        private readonly BotSetupService _botSetup;

        private float _timeSinceLastDecision;

        public BotController(List<IBotAction> availableActions, BotProfileConfig profile, BotSetupService botSetup)
        {
            _availableActions = availableActions;
            _profile = profile;
            _botSetup = botSetup;
        }

        public void Tick()
        {
            if (!_botSetup.IsBotActive) return;

            _timeSinceLastDecision += Time.deltaTime;

            if (_timeSinceLastDecision >= _profile.DecisionInterval)
            {
                _timeSinceLastDecision = 0f;

                MakeDecision();
            }
        }

        private void MakeDecision()
        {
            IBotAction bestAction = null;
            var highestScore = 0f;

            foreach (var action in _availableActions)
            {
                var score = action.Evaluate();

                if (score > highestScore)
                {
                    highestScore = score;
                    bestAction = action;
                }
            }

            if (bestAction != null && highestScore > 0f)
                bestAction.Execute();
        }
    }
}