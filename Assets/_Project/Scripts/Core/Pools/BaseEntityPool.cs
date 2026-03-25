using System.Collections.Generic;
using UnityEngine;

namespace Spawners
{
    public abstract class BaseEntityPool<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected T Prefab;

        private LinkedList<T> _activeItems;
        private LinkedList<T> _freeItems;

        protected virtual void Awake()
        {
            _activeItems = new LinkedList<T>();
            _freeItems = new LinkedList<T>();

            LoadPrefab();
        }

        protected abstract void LoadPrefab();

        public T Get()
        {
            T item;

            if (_freeItems.Count > 0)
            {
                item = _freeItems.First.Value;
                _freeItems.RemoveFirst();
            }
            else
            {
                item = Instantiate(Prefab, transform, true);
                
                OnItemCreated(item);
            }

            item.gameObject.SetActive(true);
            _activeItems.AddLast(item);

            return item;
        }

        public void ReturnToPool(T item)
        {
            item.gameObject.SetActive(false);

            _activeItems.Remove(item);
            _freeItems.AddLast(item);
        }

        protected virtual void OnItemCreated(T item)
        {
        }
    }
}