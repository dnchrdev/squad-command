using Features.GameplayECS.Physics.Systems;
using Features.GameplayECS.Physics.Systems.Ports;
using Scellecs.Morpeh;

namespace Features.GameplayECS.Physics
{
    public static class PhysicsFeature
    {
        public static void Add(SystemsGroup group, IRandomSource random)
        {
            group.AddSystem(new SnapshotVelocityForPhysicsSystem());
            group.AddSystem(new ResolveCollisionsSystem(random));
        }
    }
}