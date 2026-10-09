namespace Features.Camera.Entities
{
    public sealed class CameraSettings
    {
        public float MoveChangeFactor { get; }
        public float MoveSmoothing { get; }
        public float RotationChangeFactor { get; }
        public float MinPitch { get; }
        public float MaxPitch { get; }
        public float RotationSmoothing { get; }
        public float DistanceChangeFactor { get; }
        public float MinDistance { get; }
        public float MaxDistance { get; }
        public float DistanceSmoothing { get; }
        public float StartDistance { get; }

        public CameraSettings(
            float moveChangeFactor,
            float moveSmoothing,
            float rotationChangeFactor,
            float minPitch,
            float maxPitch,
            float rotationSmoothing,
            float distanceChangeFactor,
            float minDistance,
            float maxDistance,
            float distanceSmoothing,
            float startDistance)
        {
            MoveChangeFactor = moveChangeFactor;
            MoveSmoothing = moveSmoothing;
            RotationChangeFactor = rotationChangeFactor;
            MinPitch = minPitch;
            MaxPitch = maxPitch;
            RotationSmoothing = rotationSmoothing;
            DistanceChangeFactor = distanceChangeFactor;
            MinDistance = minDistance;
            MaxDistance = maxDistance;
            DistanceSmoothing = distanceSmoothing;
            StartDistance = startDistance;
        }
    }
}