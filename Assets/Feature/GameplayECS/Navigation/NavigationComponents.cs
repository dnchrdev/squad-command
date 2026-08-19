using Scellecs.Morpeh;
using UnityEngine;

namespace Feature.GameplayECS.Navigation
{
    public struct Destination:  IComponent
    {
        public Vector3 Target;
        public Vector3[] Corners;
        public int CurrentCornerIndex;
        public bool HasPath;
    }
    
    public struct NavigationDirection : IComponent { public Vector3 Value; }
}