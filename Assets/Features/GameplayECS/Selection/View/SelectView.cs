using System;
using UnityEngine;

namespace Features.GameplayECS.Selection.View
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