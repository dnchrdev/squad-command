using Scellecs.Morpeh;
using UnityEngine;

namespace Features.GameplayECS.Navigation.Components
{
    public struct Destination : IComponent
    {
        public Vector3 Target;
        public Vector3[] Corners;
        public int CurrentCornerIndex;
        public bool HasPath;
    }
}