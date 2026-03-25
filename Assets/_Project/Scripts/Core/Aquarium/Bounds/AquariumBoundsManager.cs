using UnityEngine;
using System;

namespace Core.Game
{
    public class AquariumBoundsManager
    {
        public Rect WorldBounds { get; private set; }

        public event Action OnBoundsUpdated;

        public void SetBounds(Rect newBounds)
        {
            WorldBounds = newBounds;
            OnBoundsUpdated?.Invoke();
        }
    }
}