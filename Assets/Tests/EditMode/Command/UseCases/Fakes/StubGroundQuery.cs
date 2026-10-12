using Features.Command.UseCases.Ports;
using UnityEngine;

namespace Tests.EditMode.Command.UseCases.Fakes
{
    public sealed class StubGroundQuery : IGroundQuery
    {
        public bool Hit = true;
        public Vector3 Point;

        public bool TryGetGroundPoint(Vector2 screenPoint, out Vector3 worldPoint)
        {
            worldPoint = Point;
            return Hit;
        }
    }
}
