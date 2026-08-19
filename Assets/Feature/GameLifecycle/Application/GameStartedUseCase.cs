using Feature.GameLifecycle.Application.Interfaces;
using UnityEngine;
using Zenject;

namespace Feature.GameLifecycle.Application
{
    public class GameStartedUseCase: IInitializable
    {
        [Inject] private readonly ICameraPositionReset _cameraPositionReset;
        [Inject] private readonly ICameraRotationReset  _cameraRotationReset;
        [Inject] private readonly ICameraBoomReset  _cameraBoomReset;

        public void Initialize()
        {
            _cameraPositionReset.SetPosition(Vector3.zero);
            _cameraRotationReset.SetRotation(Quaternion.identity);
            _cameraBoomReset.SetDistance(25f);
        }
    }
}