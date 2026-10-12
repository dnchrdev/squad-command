using Features.GameplayECS.Common.Components;
using Features.GameplayECS.Movement.Components;
using Features.GameplayECS.Movement.Outputs;
using Features.GameplayECS.Navigation.Outputs;
using Scellecs.Morpeh;

namespace Features.GameplayECS.Movement.Systems
{
    public class ApplyMoveSpeedSystem: IFixedSystem
    {
        public World World { get; set; }

        private Filter _moveUnitsFilter;

        private Stash<DesiredVelocity> _desiredVelocityStash;
        private Stash<MovementSpeed> _moveSpeedStash;
        private Stash<SteeringDirection> _steeringDirectionStash;

        public void OnAwake()
        {
            _moveUnitsFilter = World.Filter
                .With<UnitTag>()
                .With<DesiredVelocity>()
                .With<MovementSpeed>()
                .With<SteeringDirection>()
                .Build();

            _desiredVelocityStash = World.GetStash<DesiredVelocity>();
            _moveSpeedStash = World.GetStash<MovementSpeed>();
            _steeringDirectionStash = World.GetStash<SteeringDirection>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _moveUnitsFilter)
            {
                ref var desiredVelocity = ref _desiredVelocityStash.Get(entity);
                ref var movementSpeed = ref _moveSpeedStash.Get(entity);
                ref var steeringDirection = ref _steeringDirectionStash.Get(entity);

                desiredVelocity.Value += steeringDirection.Value * movementSpeed.Value;
            }
        }

        public void Dispose()
        {
        }
    }
}