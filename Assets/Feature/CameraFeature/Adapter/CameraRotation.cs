using Feature.CameraFeature.Application.Interfaces;
using Feature.CameraFeature.Infrastructure;
using UnityEngine;
using Zenject;

namespace Feature.CameraFeature.Adapter
{
    public class CameraRotation : ICameraRotation, IReadOnlyCameraOrientation
    {
        [Inject] private readonly CameraRefs _refs;

        public Vector3 Forward => _refs.RotationRoot.forward;
        public Vector3 Right => _refs.RotationRoot.right;

        public void SetRotation(Quaternion rotation)
        {
            _refs.RotationRoot.transform.rotation = rotation;
        }
    }
}