using UnityEngine;

namespace UI
{
    public abstract class BaseView : MonoBehaviour
    {
        protected bool _isInit;
        
        public bool IsInit => _isInit;
    }
}