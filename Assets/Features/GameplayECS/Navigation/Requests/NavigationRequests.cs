using System.Collections.Generic;
using Features.GameplayECS.Navigation.Components;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.GameplayECS.Navigation.Requests
{
    public struct DestinationRequest:  IComponent
    {
        public Entity Target;
        public Vector3 Destination;
    }
}