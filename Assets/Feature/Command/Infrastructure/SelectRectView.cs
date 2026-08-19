using System;
using Feature.Command.Infrastructure.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace Feature.Command.Infrastructure
{
    public class SelectRectView: MonoBehaviour, ISelectRectView
    {
        [SerializeField] private Image _selectionImage;

        private void Awake()
        {
            if(_selectionImage ==  null)
                throw new NullReferenceException("SectionImage is null");
            
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
        
        
        public void UpdateSelectionRect(Rect  selectionRect)
        {
            _selectionImage.rectTransform.position = selectionRect.position;
            _selectionImage.rectTransform.sizeDelta = new Vector2(selectionRect.width, selectionRect.height);
        }
    }
}