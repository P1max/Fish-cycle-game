using UnityEngine;

namespace UI.Core
{
    public class UIRoot : MonoBehaviour
    {
        public void SetActive(bool isActive) => gameObject.SetActive(isActive);
    }
}