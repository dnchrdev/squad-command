using Feature.GameplayECS.Common;
using Feature.GameplayECS.Navigation;
using Scellecs.Morpeh;

namespace Feature.GameplayECS.MoveCommand.Systems.Destination.Systems
{
    public class SetUnitDestinationSystem : ISystem
    {
        public World World { get; set; }
        private Filter _requestFilter;

        private Stash<UnitMoveDestinationSlot> _moveDestinationSlotStash;
        private Stash<DestinationRequest> _destinationRequestStash;
        private Stash<Position> _positionStash;
        private Stash<MoveDestinationFormation> _destinationFormationStash;
        private Stash<ApplyDestinationFormationSelfRequest> _applyDestinationSelfRequestStash;

        public void OnAwake()
        {
            _requestFilter = World.Filter
                .With<MoveDestinationFormation>()
                //.With<FormationUnits>()
                .With<DestinationSlotsAssignedTag>()
                .With<ApplyDestinationFormationSelfRequest>()
                .Build();

            _moveDestinationSlotStash = World.GetStash<UnitMoveDestinationSlot>();
            _positionStash = World.GetStash<Position>();
            _applyDestinationSelfRequestStash = World.GetStash<ApplyDestinationFormationSelfRequest>();
            _destinationRequestStash = World.GetStash<DestinationRequest>();
            _destinationFormationStash = World.GetStash<MoveDestinationFormation>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var requestFormation in _requestFilter)
            {
                var formationUnitsComp = _destinationFormationStash.Get(requestFormation);
                foreach (var unit in formationUnitsComp.Units)
                {
                    var unitSlotComp = _moveDestinationSlotStash.Get(unit);
                    var slotPositionComp = _positionStash.Get(unitSlotComp.Slot);

                    _destinationRequestStash.Set(World.CreateEntity(),
                        new DestinationRequest { Target = unit, Destination = slotPositionComp.Value });
                }

                _applyDestinationSelfRequestStash.Remove(requestFormation);
            }
        }

        public void Dispose()
        {
        }
    }
}