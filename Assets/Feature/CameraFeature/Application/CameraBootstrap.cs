using Feature.CameraFeature.Application.Interfaces;
using Feature.CameraFeature.Domain;
using Zenject;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;

namespace Feature.CameraFeature.Application
{
    public class CameraBootstrap
    {
        [Inject] private readonly CameraOrientation _orientation;
        [Inject] private readonly CameraPosition _position;
        [Inject] private readonly CameraDistance _distance;

        [Inject] private readonly ICameraRotation _cameraRotation;
        [Inject] private readonly ICameraPosition _cameraPosition;
        [Inject] private readonly ICameraBoom _cameraBoom;

        public void GameStarted()
        {
            _orientation.Reset(Quaternion.identity);
            _position.Reset(Vector3.zero);
            _distance.Reset(25f);

            _cameraRotation.SetRotation(_orientation.Current);
            _cameraPosition.SetPosition(_position.Current);
            _cameraBoom.SetDistance(_distance.Current);
        }
    }
}