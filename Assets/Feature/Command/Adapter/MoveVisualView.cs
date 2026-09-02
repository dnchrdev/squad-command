using Feature.Command.Application.Interfaces;
using Feature.Shared.DevTools;
using UnityEngine;
using UnityEngine.UI;

namespace Feature.Command.Adapter
{
    [RequireComponent(typeof(CanvasScaler))]
    public class MoveVisualView : MonoBehaviour, IMoveVisualView
    {
        [SerializeField] private GameObject _moveStartPoint;
        [SerializeField] private GameObject _moveEndPoint;
        [SerializeField] private RectTransform _moveLinker;

        private CanvasScaler _canvassScaler;

        private void OnValidate()
        {
            InspectorRefValidator.CheckAllAssigned(this,
                (_moveStartPoint, nameof(_moveStartPoint)),
                (_moveEndPoint, nameof(_moveEndPoint)),
                (_moveLinker, nameof(_moveLinker)));
        }

        public void Initialize()
        {
            _canvassScaler = GetComponent<CanvasScaler>();
            Hide();
        }
        

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public Vector2 GetScreenScaleFactor()
        {
            float heightFactor = _canvassScaler.referenceResolution.y * 1f / Screen.height;
            float widthFactor = _canvassScaler.referenceResolution.x * 1f / Screen.width;

            return new Vector2(widthFactor, heightFactor);
        }

        public void SetSelectionStart(Vector2 position)
        {
            if (_moveStartPoint == null) return;

            _moveStartPoint.transform.position = position;
        }

        public void UpdateSelectionEnd(Vector2 position)
        {
            if (_moveEndPoint == null) return;

            _moveEndPoint.transform.position = position;
        }

        public void SetLinkerPosition(Vector2 position)
        {
            if (_moveLinker == null) return;

            _moveLinker.transform.position = position;
        }

        public void SetLinkerZRotation(float zRotation)
        {
            if (_moveLinker == null) return;

            _moveLinker.transform.rotation = Quaternion.Euler(0f, 0f, zRotation);
        }

        public void SetLinkerLength(float lentgh)
        {
            if (_moveLinker == null) return;

            _moveLinker.sizeDelta = new Vector2(_moveLinker.sizeDelta.x, lentgh);
        }
    }
}