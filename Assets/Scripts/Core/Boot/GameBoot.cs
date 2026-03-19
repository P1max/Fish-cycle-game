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
            _fishPool.GetFish("goldfish_basic");
            _fishPool.GetFish("pinkfish_fit");
            _fishPool.GetFish("bluefish_fat");
        }
    }
}