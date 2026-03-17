using Spawners;
using UnityEngine;
using Zenject;

namespace Core.Boot
{
    public class GameBoot : MonoBehaviour
    {
        [Inject] private FishPool _fishPool;

        private void Start()
        {
            _fishPool.GetFish();
            _fishPool.GetFish();
            _fishPool.GetFish();
        }
    }
}