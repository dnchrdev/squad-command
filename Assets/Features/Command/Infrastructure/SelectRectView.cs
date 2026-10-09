using Features.Command.InterfaceAdapters.Ports;
using Features.Shared.DevTools;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Command.Infrastructure
{
    public class SelectRectView : MonoBehaviour, ISelectRectView
    {
        [SerializeField] private Image _selectionImage;

        private void OnValidate()
        {
            InspectorRefValidator.CheckAssigned(_selectionImage, nameof(_selectionImage), this);
        }

        public void Initialize() => Hide();
        public void Show() => _selectionImage.gameObject.SetActive(true);
        public void Hide() => _selectionImage.gameObject.SetActive(false);

        public void UpdateSelectionRect(Rect rect)
        {
            _selectionImage.rectTransform.position = rect.position;
            _selectionImage.rectTransform.sizeDelta = new Vector2(rect.width, rect.height);
        }
    }
}