using System.Collections.Generic;
using Scellecs.Morpeh;
using UnityEngine;

namespace Feature.GameplayECS.Navigation
{
    public struct DestinationRequest:  IComponent
    {
        public Entity Target;
        public Vector3 Destination;
    }

    public struct MoveOrderRequest : IComponent
    {
        public List<Vector3> TargetPoints;
        public List<Entity> Units;
    }
    
}