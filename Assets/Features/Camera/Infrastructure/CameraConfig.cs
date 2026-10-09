using Features.Camera.Entities;
using UnityEngine;

namespace Features.Camera.Infrastructure
{
    [CreateAssetMenu(fileName = "CameraConfig", menuName = "Scriptable Objects/CameraConfig")]
    public class CameraConfig : ScriptableObject
    {
        [field: Header("Move")]
        [field: SerializeField, Range(0.01f, 3)] public float MoveChangeFactor { get; private set; }
        [field: SerializeField, Range(0.01f, 100)] public float MoveSmoothing { get; private set; }
    
        [field: Header("Rotation")]
        [field: SerializeField, Range(0.01f, 3f)] public float RotationChangeFactor { get; private set; }
        [field: SerializeField, Range(0f, 89)] public float MaxPitch  { get; private set; }
        [field: SerializeField, Range(0f, 89)] public float MinPitch { get; private set; }
        [field: SerializeField, Range(0.01f, 100)] public float RotationSmoothing { get; private set; }
    
        [field: Header("Boom")]
        [field: SerializeField, Range(1f, 5)] public float DistanceChangeFactor { get; private set; }
        [field: SerializeField, Range(5f, 50)] public float MaxDistance  { get; private set; }
        [field: SerializeField, Range(5f, 50)] public float MinDistance{ get; private set; }
        [field: SerializeField, Range(0.01f, 100)] public float DistanceSmoothing { get; private set; }
        [field: SerializeField, Range(5f, 50)] public float StartDistance { get; private set; } = 25f;

        public CameraSettings ToSettings() => new CameraSettings(
            MoveChangeFactor,
            MoveSmoothing,
            RotationChangeFactor,
            MinPitch,
            MaxPitch,
            RotationSmoothing,
            DistanceChangeFactor,
            MinDistance,
            MaxDistance,
            DistanceSmoothing,
            StartDistance);

        private void OnValidate()
        {
            if(MinPitch > MaxPitch) MinPitch = MaxPitch;
            if (MinDistance > MaxDistance) MinDistance = MaxDistance;
        }
    }
}
