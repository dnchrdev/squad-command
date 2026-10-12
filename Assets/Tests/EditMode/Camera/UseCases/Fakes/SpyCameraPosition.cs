using Features.Camera.UseCases.Ports;
using UnityEngine;

namespace Tests.EditMode.Camera.UseCases.Fakes
{
    public sealed class SpyCameraPosition : ICameraPosition
    {
        public Vector3 LastPosition;
        public int Calls;

        public void SetPosition(Vector3 position)
        {
            LastPosition = position;
            Calls++;
        }
    }
}
