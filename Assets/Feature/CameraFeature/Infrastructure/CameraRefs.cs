using System;
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
            if(PositionRoot == null) throw  new NullReferenceException("PositionRoot is null");
            if(RotationRoot == null) throw new NullReferenceException("RotationRoot is null");
            if(BoomRoot == null) throw  new NullReferenceException("BoomRoot is null");
        }
    }
}