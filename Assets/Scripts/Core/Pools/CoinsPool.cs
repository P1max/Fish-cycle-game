using System.Collections.Generic;
using Core.Coin;
using Core.Game;
using UnityEngine;
using Zenject;

namespace Spawners
{
    public class CoinsPool : MonoBehaviour
    {
        [Inject] private BalanceManager _balanceManager;
        
        private Dictionary<Collider2D, Coin> _coinsCache;
        private LinkedList<Coin> _activeCoins;
        private LinkedList<Coin> _freeCoins;
        private Coin _coinPrefab;
        
        public IReadOnlyDictionary<Collider2D, Coin> CoinsCache => _coinsCache;

        private void Awake()
        {
            _coinPrefab = Resources.Load<Coin>("Prefabs/Coin");

            _activeCoins = new LinkedList<Coin>();
            _freeCoins = new LinkedList<Coin>();
            _coinsCache = new Dictionary<Collider2D, Coin>();
        }

        public Coin GetCoin(Vector3 position, int value)
        {
            Coin coin;

            if (_freeCoins.Count > 0)
            {
                coin = _freeCoins.First.Value;
                _freeCoins.RemoveFirst();
            }
            else
            {
                coin = Instantiate(_coinPrefab, transform, true);

                coin.Init(_balanceManager, this);

                var col = coin.GetComponent<Collider2D>();

                _coinsCache.Add(col, coin);
            }

            coin.gameObject.SetActive(true);
            coin.Spawn(position, value);

            _activeCoins.AddLast(coin);

            return coin;
        }

        public void ReturnCoin(Coin coin)
        {
            coin.gameObject.SetActive(false);

            _activeCoins.Remove(coin);
            _freeCoins.AddLast(coin);
        }
    }
}