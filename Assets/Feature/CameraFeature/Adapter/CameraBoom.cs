using Feature.CameraFeature.Application.Interfaces;
using Feature.CameraFeature.Infrastructure;
using UnityEngine;
using Zenject;

namespace Feature.CameraFeature.Adapter
{
    public class CameraBoom : ICameraBoom
    {
        [Inject] private readonly CameraRefs _refs;

        public void SetDistance(float distance)
        {
            _refs.BoomRoot.transform.localPosition = new Vector3(0f, 0f, -distance);
        }
    }
}