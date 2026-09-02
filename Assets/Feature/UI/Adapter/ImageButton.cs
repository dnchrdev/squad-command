using System;
using R3;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Feature.UI.Adapter
{
    public class ImageButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler,
        IPointerDownHandler, IPointerUpHandler
    {
        private readonly Subject<Unit> _onClick = new Subject<Unit>();
        private readonly Subject<Unit> _onDown = new Subject<Unit>();
        private readonly Subject<Unit> _onUp = new Subject<Unit>();

        public Observable<Unit> Click => _onClick;
        public Observable<Unit> Down => _onDown;
        public Observable<Unit> Up => _onUp;

        [SerializeField] private GameObject _pointerHighlight;


        private void Awake()
        {
            Initialize();

#if UNITY_EDITOR
            CheckForNull();
#endif
        }

        protected virtual void Initialize()
        {
        }

        protected virtual void CheckForNull()
        {
            if (_pointerHighlight == null)
                throw new NullReferenceException("Button had null pointer highlight");
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _onClick?.OnNext(Unit.Default);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _onDown?.OnNext(Unit.Default);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_pointerHighlight != null)
                _pointerHighlight?.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_pointerHighlight != null)
                _pointerHighlight?.SetActive(false);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _onUp?.OnNext(Unit.Default);
        }
    }
}