using Features.Camera.UseCases.Ports;
using UnityEngine;

namespace Tests.EditMode.Camera.UseCases.Fakes
{
    public sealed class SpyCameraRotation : ICameraRotation
    {
        public Quaternion LastRotation;
        public int Calls;

        public void SetRotation(Quaternion rotation)
        {
            LastRotation = rotation;
            Calls++;
        }
    }
}
