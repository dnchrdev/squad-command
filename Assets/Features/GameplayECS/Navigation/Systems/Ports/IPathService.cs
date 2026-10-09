using UnityEngine;

namespace Features.GameplayECS.Navigation.Systems.Ports
{
    public interface IPathService
    {
        bool TryBuildPath(Vector3 from, Vector3 requested, out Vector3 target, out Vector3[] corners);
    }
}