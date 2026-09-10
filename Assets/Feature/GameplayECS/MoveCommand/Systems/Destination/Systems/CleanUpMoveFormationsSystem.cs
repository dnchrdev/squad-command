using Scellecs.Morpeh;

namespace Feature.GameplayECS.MoveCommand.Systems.Destination.Systems
{
    public class CleanUpMoveFormationsSystem : ISystem
    {
        public World World { get; set; }

        private Filter _activeMoveFormationsFilter;

        private Stash<MoveDestinationFormation> _moveDestinationFormationStash;
        private Stash<DestinationSlotsAssignedTag> _destinationSlotsAssignedTagStash;


        public void OnAwake()
        {
            _activeMoveFormationsFilter = World.Filter
                .With<MoveDestinationFormation>()
                .With<DestinationSlotsAssignedTag>()
                .Without<ApplyDestinationFormationSelfRequest>()
                .Build();

            _moveDestinationFormationStash = World.GetStash<MoveDestinationFormation>();
            _destinationSlotsAssignedTagStash =  World.GetStash<DestinationSlotsAssignedTag>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var formation in _activeMoveFormationsFilter)
            {
                ref var destinationFormationComp = ref _moveDestinationFormationStash.Get(formation);
        
                if (destinationFormationComp.Slots.Count == 0 || destinationFormationComp.Units.Count == 0)
                {
                    _moveDestinationFormationStash.Remove(formation);
                    _destinationSlotsAssignedTagStash.Remove(formation);
                    
                    World.RemoveEntity(formation);
                }
            }
        }

        public void Dispose()
        {
        }
    }
}