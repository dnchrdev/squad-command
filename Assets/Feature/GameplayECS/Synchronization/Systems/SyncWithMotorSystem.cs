using System;
using Feature.GameplayECS.Common;
using Feature.GameplayECS.Movement;
using Scellecs.Morpeh;

namespace Feature.GameplayECS.Synchronization.Systems
{
    public class SyncWithMotorSystem: ISystem
    {
        public World World { get; set; }

        private Filter _allUnitsFilter;
        
        private Stash<Position> _positionStash;
        private Stash<Rotation> _rotationStash;
        private Stash<UnitMotorComponent> _motorStash;
        
        public void OnAwake()
        {
            _allUnitsFilter = World.Filter
                .With<Position>()
                .With<Rotation>()
                .With<UnitMotorComponent>()
                .Build();

            _positionStash = World.GetStash<Position>();
            _rotationStash = World.GetStash<Rotation>();
            _motorStash = World.GetStash<UnitMotorComponent>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var unit in _allUnitsFilter)
            {
                ref var motor = ref _motorStash.Get(unit);

                if (motor.Value == null) throw new NullReferenceException("UnitMotorComponent.Slot is null");
                
                _positionStash.Set(unit, new Position{Value = motor.Value.Position});
                _rotationStash.Set(unit, new Rotation{Value = motor.Value.Rotation});
            }
        }
        
        public void Dispose()
        {
            
        }
    }
}