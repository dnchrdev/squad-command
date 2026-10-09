using UnityEngine;

namespace Features.Shared.Projection
{
    public interface IScreenProjection
    {
        Vector3 WorldToScreenPoint(Vector3 worldPoint);
        Ray ScreenPointToRay(Vector2 screenPoint);
    }
}