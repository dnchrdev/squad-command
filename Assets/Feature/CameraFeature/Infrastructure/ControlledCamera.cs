using System;
using Feature.CameraFeature.Infrastructure.Interfaces;
using UnityEngine;

namespace Feature.CameraFeature.Infrastructure
{
    [RequireComponent(typeof(Camera))]
    public class ControlledCamera : MonoBehaviour, IReadOnlyCamera
    {
        private Camera _camera;

        public Camera Camera => _camera;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
        }
    }
}