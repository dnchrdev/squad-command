using UnityEngine;

namespace Features.Camera.Infrastructure
{
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class ControlledCamera : MonoBehaviour
    {
        private UnityEngine.Camera _camera;

        public UnityEngine.Camera Camera => _camera;

        private void Awake()
        {
            _camera = GetComponent<UnityEngine.Camera>();
        }
    }
}