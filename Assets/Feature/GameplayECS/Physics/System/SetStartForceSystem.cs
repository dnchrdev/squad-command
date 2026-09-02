using Feature.GameplayECS.Common;
using Feature.GameplayECS.Movement;
using Scellecs.Morpeh;

namespace Feature.GameplayECS.Physics.System
{
    public class SnapshotVelocityForPhysicsSystem : IFixedSystem
    {
        public World World { get; set; }
 
        private Filter _bodiesFilter;
 
        private Stash<Velocity> _velocityStash;
        private Stash<PhysicsForce> _forceStash;
 
        public void OnAwake()
        {
            _bodiesFilter = World.Filter
                .With<UnitTag>()
                .With<Velocity>()
                .With<PhysicsForce>()
                .Build();
 
            _velocityStash = World.GetStash<Velocity>();
            _forceStash = World.GetStash<PhysicsForce>();
        }
 
        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _bodiesFilter)
            {
                ref var velocity = ref _velocityStash.Get(entity);
                ref var force = ref _forceStash.Get(entity);
 
                force.Value = velocity.Value;
            }
        }
 
        public void Dispose()
        {
        }
    }
}