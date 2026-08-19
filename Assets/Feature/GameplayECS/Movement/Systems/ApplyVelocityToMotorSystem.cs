using Feature.GameplayECS.Common;
using Scellecs.Morpeh;

namespace Feature.GameplayECS.Movement.Systems
{
    public class ApplyVelocityToMotorSystem: IFixedSystem
    {
        public World World { get; set; }

        private Filter _moveUnitsFilter;

        private Stash<Velocity> _velocityStash;
        private Stash<UnitMotorComponent> _motorStash;

        public void OnAwake()
        {
            _moveUnitsFilter = World.Filter
                .With<UnitTag>()
                .With<Velocity>()
                .With<UnitMotorComponent>()
                .Build();

            _velocityStash = World.GetStash<Velocity>();
            _motorStash = World.GetStash<UnitMotorComponent>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _moveUnitsFilter)
            {
                ref var velocity = ref _velocityStash.Get(entity);
                ref var motor = ref _motorStash.Get(entity);

                motor.Value.SetVelocity(velocity.Value);
            }
        }

        public void Dispose()
        {
        }
    }
}