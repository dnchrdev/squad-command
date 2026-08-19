using System;
using Feature.GameplayECS.View;
using Scellecs.Morpeh;
using UnityEngine;

namespace Feature.GameplayECS.Select
{
    public class SelectView : MonoBehaviour
    {
        [SerializeField] private GameObject _selectedVisual;

        private void Awake()
        {
            if(_selectedVisual == null) throw new NullReferenceException("SelectedVisual is null");
            
            Hide();
        }

        public void Show()
        {
            _selectedVisual.SetActive(true);
        }

        public void Hide()
        {
            _selectedVisual.SetActive(false);
        }
    }
}