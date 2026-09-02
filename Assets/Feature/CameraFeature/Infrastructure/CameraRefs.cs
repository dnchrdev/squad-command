using System;
using Feature.Shared.DevTools;
using UnityEngine;

namespace Feature.CameraFeature.Infrastructure
{
    public class CameraRefs : MonoBehaviour
    {
        [field: SerializeField] public Transform PositionRoot { get; private set; }
        [field: SerializeField] public Transform RotationRoot { get; private set; }
        [field: SerializeField] public Transform BoomRoot { get; private set; }

        private void OnValidate()
        {
            InspectorRefValidator.CheckAssigned(PositionRoot, nameof(PositionRoot), this);
            InspectorRefValidator.CheckAssigned(RotationRoot, nameof(RotationRoot), this);
            InspectorRefValidator.CheckAssigned(BoomRoot, nameof(BoomRoot), this);
        }
    }
}