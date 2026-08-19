namespace Feature.CameraFeature.Adapter.Interfaces
{
    public interface ICameraBoomUpdate
    {
        void AddDistance(float zoomValue);
        void Tick(float dt);
    }
}