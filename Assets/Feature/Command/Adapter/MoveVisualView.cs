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

        private CanvasScaler _canvasScaler;

        private void OnValidate()
        {
            InspectorRefValidator.CheckAssigned(_moveStartPoint, nameof(_moveStartPoint), this);
            InspectorRefValidator.CheckAssigned(_moveEndPoint, nameof(_moveEndPoint), this);
            InspectorRefValidator.CheckAssigned(_moveLinker, nameof(_moveLinker), this);
        }

        public void Initialize()
        {
            _canvasScaler = GetComponent<CanvasScaler>();
            Hide();
        }

        public void Show() => gameObject.SetActive(true);
        public void Hide() => gameObject.SetActive(false);

        public void SetSelectionStart(Vector2 screenPos)
        {
            _moveStartPoint.transform.position = screenPos;
            _moveEndPoint.transform.position = screenPos;
            _moveLinker.position = screenPos;
            _moveLinker.sizeDelta = new Vector2(_moveLinker.sizeDelta.x, 0f);
        }

        public void UpdateDrag(Vector2 screenStart, Vector2 screenEnd)
        {
            var delta = screenEnd - screenStart;
            _moveLinker.rotation = Quaternion.Euler(0f, 0f, LinkerGeometry.ComputeZRotation(delta));
            _moveLinker.sizeDelta = new Vector2(_moveLinker.sizeDelta.x,
                LinkerGeometry.ComputeLength(delta, GetScaleFactor()));
            _moveEndPoint.transform.position = screenEnd;
        }

        private Vector2 GetScaleFactor() => new Vector2(
            _canvasScaler.referenceResolution.x / Screen.width,
            _canvasScaler.referenceResolution.y / Screen.height);
    }
}