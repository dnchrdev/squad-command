using Features.GameplayECS.Common.Components;
using Features.GameplayECS.Movement.Components;
using Features.GameplayECS.Movement.Outputs;
using Features.GameplayECS.Navigation.Outputs;
using Features.GameplayECS.Physics.Components;
using Features.GameplayECS.Physics.Outputs;
using Features.GameplayECS.Synchronization.Outputs;
using Features.GameplayECS.UnitView.Components;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.GameplayECS.Spawning.Systems
{
    public class SpawnOnInitInitializer: IInitializer
    {
        public World World { get; set; }
        
        private Stash<MovementSpeed> _movementSpeedStash;
        private Stash<UnitTag>  _unitTagStash;
        private Stash<AssetPath>  _assetPathStash;
        private Stash<Position>  _positionStash;
        private Stash<Rotation>   _rotationStash;
        private Stash<Velocity>  _velocityStash;    
        private Stash<SteeringDirection> _steeringDirectionStash;
        private Stash<DesiredVelocity> _desiredVelocityStash;
        private Stash<CollisionRadius> _collisionRadiusStash;
        private Stash<PlayerControlledTag> _playerControlledStash;
        
        public void OnAwake()
        {
            _unitTagStash = World.GetStash<UnitTag>();
            _movementSpeedStash = World.GetStash<MovementSpeed>();
            _assetPathStash  = World.GetStash<AssetPath>();
            _positionStash = World.GetStash<Position>();
            _rotationStash = World.GetStash<Rotation>();    
            _velocityStash = World.GetStash<Velocity>();
            _steeringDirectionStash = World.GetStash<SteeringDirection>();
            _desiredVelocityStash = World.GetStash<DesiredVelocity>();
            _collisionRadiusStash = World.GetStash<CollisionRadius>();
            _playerControlledStash = World.GetStash<PlayerControlledTag>();

            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    var unit = World.CreateEntity();
                    NewUnit(ref unit, Vector3.right * i + Vector3.forward * j);
                }
            }
        }

        private void NewUnit(ref Entity unit, Vector3 position)
        {
            _unitTagStash.Add(unit);
            _movementSpeedStash.Add(unit, new MovementSpeed{Value = 3.5f});
            _assetPathStash.Add(unit, new AssetPath{Value = "Unit"});
            _positionStash.Add(unit, new Position {Value = Vector3.zero + Vector3.up * 1 + position});
            _rotationStash.Add(unit, new Rotation {Value = Quaternion.identity});
            _velocityStash.Add(unit, new Velocity {Value = Vector3.zero});
            _steeringDirectionStash.Add(unit, new SteeringDirection {Value = Vector3.zero});
            _desiredVelocityStash.Add(unit, new DesiredVelocity {Value = Vector3.zero});
            _collisionRadiusStash.Add(unit, new CollisionRadius {Value = 0.5f});
            _playerControlledStash.Add(unit);
        }

        public void Dispose()
        {
            
        }
    }
}