using Feature.CameraFeature.Application.Interfaces;
using Feature.CameraFeature.Infrastructure;
using UnityEngine;
using Zenject;

namespace Feature.CameraFeature.Adapter
{
    public class CameraPosition : ICameraPosition
    {
        [Inject] private readonly CameraRefs _refs;

        public void SetPosition(Vector3 position)
        {
            _refs.PositionRoot.transform.position = position;
        }
    }
}