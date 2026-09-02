using Feature.Command.Application.Interfaces;
using Feature.Shared.DevTools;
using UnityEngine;
using UnityEngine.UI;

namespace Feature.Command.Adapter
{
    public class SelectRectView : MonoBehaviour, ISelectRectView
    {
        [SerializeField] private Image _selectionImage;

        private void OnValidate()
        {
            InspectorRefValidator.CheckAssigned(_selectionImage, nameof(_selectionImage), this);
        }

        public void Initialize()
        {
            Hide();
        }

        public void Show()
        {
            _selectionImage.gameObject.SetActive(true);
        }

        public void Hide()
        {
            _selectionImage.gameObject.SetActive(false);
        }


        public void UpdateSelectionRect(Rect selectionRect)
        {
            _selectionImage.rectTransform.position = selectionRect.position;
            _selectionImage.rectTransform.sizeDelta = new Vector2(selectionRect.width, selectionRect.height);
        }
    }
}