using Features.GameplayECS.Navigation.Systems.Ports;
using UnityEngine;

namespace Tests.EditMode.GameplayECS.Navigation.Fakes
{
    public sealed class FakePathService : IPathService
    {
        public bool Result;
        public Vector3 Target;
        public Vector3[] Corners;

        public Vector3 LastFrom;
        public Vector3 LastRequested;

        public bool TryBuildPath(Vector3 from, Vector3 requested, out Vector3 target, out Vector3[] corners)
        {
            LastFrom = from;
            LastRequested = requested;
            target = Target;
            corners = Corners;
            return Result;
        }
    }
}