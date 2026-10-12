using Features.Camera.UseCases.Ports;

namespace Tests.EditMode.Camera.UseCases.Fakes
{
    public sealed class SpyCameraBoom : ICameraBoom
    {
        public float LastDistance;
        public int Calls;

        public void SetDistance(float distance)
        {
            LastDistance = distance;
            Calls++;
        }
    }
}
