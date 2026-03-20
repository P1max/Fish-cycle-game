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
            _fishPool.GetFish("goldfish_basic").transform.position = Vector2.zero;
            _fishPool.GetFish("pinkfish_fit").transform.position = Vector2.zero;
            _fishPool.GetFish("bluefish_fat").transform.position = Vector2.zero;
        }
    }
}