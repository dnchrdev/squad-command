using Features.GameplayECS.Common.Components;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.GameplayECS.Movement.Systems
{
    public class ResetVelocitySystem: IFixedSystem
    {
        public World World { get; set; }

        private Filter _allUnitsFilter;
        
        private Stash<Velocity> _velocityStash;
        
        public void OnAwake()
        {
            _allUnitsFilter = World.Filter
                .With<UnitTag>()
                .With<Velocity>()
                .Build();

            _velocityStash = World.GetStash<Velocity>();
        }
        
        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _allUnitsFilter)
            {
                ref var velocity = ref _velocityStash.Get(entity);
                
                velocity.Value = Vector3.zero;
            }
        }
        
        public void Dispose()
        {
            
        }
    }
}