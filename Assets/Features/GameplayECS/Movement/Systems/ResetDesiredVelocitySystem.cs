using Features.GameplayECS.Common.Components;
using Features.GameplayECS.Movement.Outputs;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.GameplayECS.Movement.Systems
{
    public class ResetDesiredVelocitySystem: IFixedSystem
    {
        public World World { get; set; }

        private Filter _allUnitsFilter;
        
        private Stash<DesiredVelocity> _desiredVelocityStash;
        
        public void OnAwake()
        {
            _allUnitsFilter = World.Filter
                .With<UnitTag>()
                .With<DesiredVelocity>()
                .Build();

            _desiredVelocityStash = World.GetStash<DesiredVelocity>();
        }
        
        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _allUnitsFilter)
            {
                ref var desiredVelocity = ref _desiredVelocityStash.Get(entity);
                
                desiredVelocity.Value = Vector3.zero;
            }
        }
        
        public void Dispose()
        {
            
        }
    }
}