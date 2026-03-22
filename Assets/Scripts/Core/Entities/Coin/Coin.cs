using Core.Game;
using Spawners;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace Core.Coin
{
    [RequireComponent(typeof(Collider2D))]
    public class Coin : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private BalanceManager _balanceManager;
        private Sequence _collectSequence;
        private Collider2D _collider;
        private CoinsPool _pool;
        private int _coinValue;
        private bool _isCollected;

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
        }

        private void OnMouseDown()
        {
#if UNITY_ANDROID || UNITY_IOS
            if (Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) return;
#else
            if (EventSystem.current.IsPointerOverGameObject()) return;
#endif

            if (_isCollected) return;

            _isCollected = true;

            _balanceManager.AddCoins(_coinValue);

            PlayCollectAnimation();
        }

        private void PlayCollectAnimation()
        {
            _collider.enabled = false;
            
            _collectSequence = DOTween.Sequence()
                .Append(transform.DOMoveY(transform.position.y + 1.5f, 0.5f).SetEase(Ease.OutCubic))
                .Join(transform.DORotate(new Vector3(0, 360f, 0), 0.5f, RotateMode.FastBeyond360).SetEase(Ease.InOutSine))
                .Join(_spriteRenderer.DOFade(0f, 0.5f).SetEase(Ease.InQuad))
                .OnComplete(() => _pool.ReturnCoin(this));
        }

        public void Spawn(Vector2 position, int value)
        {
            _isCollected = false;
            _coinValue = value;
            transform.position = position;
            _collider.enabled = true;

            _collectSequence?.Kill();
            transform.DOKill();

            transform.rotation = Quaternion.identity;
            _spriteRenderer.color = Color.white;
        }

        public void Init(BalanceManager balanceManager, CoinsPool pool)
        {
            _balanceManager = balanceManager;
            _pool = pool;
        }
    }
}