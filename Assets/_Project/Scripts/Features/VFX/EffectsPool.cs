using System.Collections.Generic;
using Core.Entities.VFX;
using UnityEngine;
using Zenject;

namespace Spawners
{
    public class EffectsPool : MonoBehaviour
    {
        [Inject] private AquariumConfig _aquariumConfig;

        private Dictionary<string, BaseVFXEntity> _prefabsMap;
        private Dictionary<string, Queue<BaseVFXEntity>> _freeItems;

        private void Awake()
        {
            transform.localScale = Vector3.one * _aquariumConfig.DefaultEntitiesScale * _aquariumConfig.FishesDefaultScale;

            _prefabsMap = new Dictionary<string, BaseVFXEntity>();
            _freeItems = new Dictionary<string, Queue<BaseVFXEntity>>();

            LoadPrefabsFromResources();
        }

        private void LoadPrefabsFromResources()
        {
            var loadedPrefabs = Resources.LoadAll<BaseVFXEntity>("Prefabs/VFX");

            foreach (var prefab in loadedPrefabs)
            {
                if (string.IsNullOrEmpty(prefab.Id))
                {
                    Debug.LogError($"У префаба '{prefab.name}' не задан Id в инспекторе!");

                    continue;
                }

                if (_prefabsMap.ContainsKey(prefab.Id))
                {
                    Debug.LogError($"Найден дубликат Id '{prefab.Id}' у префаба '{prefab.name}'!");

                    continue;
                }

                _prefabsMap[prefab.Id] = prefab;
                _freeItems[prefab.Id] = new Queue<BaseVFXEntity>();
            }
        }

        public BaseVFXEntity SpawnEffect(string effectId, Vector2 position)
        {
            if (!_prefabsMap.TryGetValue(effectId, out var prefab))
            {
                Debug.LogError($"Эффект с ID '{effectId}' не найден!");

                return null;
            }

            BaseVFXEntity effect;
            var queue = _freeItems[effectId];

            if (queue.Count > 0)
            {
                effect = queue.Dequeue();
            }
            else
            {
                effect = Instantiate(prefab, transform, false);
                effect.Init(this);
            }

            effect.gameObject.SetActive(true);
            effect.Play(position);

            return effect;
        }

        public void ReturnToPool(BaseVFXEntity effect)
        {
            effect.gameObject.SetActive(false);
            _freeItems[effect.Id].Enqueue(effect);
        }
    }
}