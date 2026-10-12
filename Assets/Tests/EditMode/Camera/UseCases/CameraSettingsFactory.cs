using Features.Camera.Entities;

namespace Tests.EditMode.Camera.UseCases
{
    public static class CameraSettingsFactory
    {
        public const float MOVE_CHANGE_FACTOR = 2f;
        public const float MOVE_SMOOTHING = 10f;
        public const float ROTATION_CHANGE_FACTOR = 2f;
        public const float MIN_PITCH = -80f;
        public const float MAX_PITCH = 80f;
        public const float ROTATION_SMOOTHING = 10f;
        public const float DISTANCE_CHANGE_FACTOR = 3f;
        public const float MIN_DISTANCE = 5f;
        public const float MAX_DISTANCE = 50f;
        public const float DISTANCE_SMOOTHING = 10f;
        public const float START_DISTANCE = 25f;

        public static CameraSettings Create() => new CameraSettings(
            MOVE_CHANGE_FACTOR,
            MOVE_SMOOTHING,
            ROTATION_CHANGE_FACTOR,
            MIN_PITCH,
            MAX_PITCH,
            ROTATION_SMOOTHING,
            DISTANCE_CHANGE_FACTOR,
            MIN_DISTANCE,
            MAX_DISTANCE,
            DISTANCE_SMOOTHING,
            START_DISTANCE);
    }
}
