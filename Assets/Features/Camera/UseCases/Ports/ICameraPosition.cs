using UnityEngine;

namespace Features.Camera.UseCases.Ports
{
    public interface ICameraPosition
    {
        void SetPosition(Vector3 position);
    }
}