using Features.GameplayECS.Synchronization.Systems;
using Scellecs.Morpeh;

namespace Features.GameplayECS.Synchronization
{
    public static class SynchronizationFeature
    {
        public static void AddReadFromUnity(SystemsGroup group)
        {
            group.AddSystem(new SyncWithMotorSystem());
        }

        public static void AddWriteToUnity(SystemsGroup group)
        {
            group.AddSystem(new ApplyVelocityToMotorSystem());
        }
    }
}