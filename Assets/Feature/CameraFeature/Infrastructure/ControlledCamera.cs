using System;
using UnityEngine;

namespace Feature.CameraFeature.Infrastructure
{
    public class ControlledCamera : MonoBehaviour, IReadOnlyCamera
    {
        private Camera _camera;

        public Camera Camera => _camera;

        private void Awake()
        {
            _camera = GetComponent<Camera>();

            if (_camera == null)
                throw new NullReferenceException("Script was not attached to a Camera");
        }
    }
}