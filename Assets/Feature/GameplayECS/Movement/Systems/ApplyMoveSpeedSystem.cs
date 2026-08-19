using Feature.GameplayECS.Common;
using Feature.GameplayECS.Navigation;
using Scellecs.Morpeh;

namespace Feature.GameplayECS.Movement.Systems
{
    public class ApplyMoveSpeedSystem: ISystem
    {
        public World World { get; set; }

        private Filter _moveUnitsFilter;

        private Stash<Velocity> _velocityStash;
        private Stash<MovementSpeed> _moveSpeedStash;
        private Stash<NavigationDirection> _navigationDirectionStash;

        public void OnAwake()
        {
            _moveUnitsFilter = World.Filter
                .With<UnitTag>()
                .With<Velocity>()
                .With<MovementSpeed>()
                .With<NavigationDirection>()
                .Build();

            _velocityStash = World.GetStash<Velocity>();
            _moveSpeedStash = World.GetStash<MovementSpeed>();
            _navigationDirectionStash = World.GetStash<NavigationDirection>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _moveUnitsFilter)
            {
                ref var velocity = ref _velocityStash.Get(entity);
                ref var movementSpeed = ref _moveSpeedStash.Get(entity);
                ref var navigationDirection = ref _navigationDirectionStash.Get(entity);

                velocity.Value += navigationDirection.Value * movementSpeed.Value;
            }
        }

        public void Dispose()
        {
        }
    }
}