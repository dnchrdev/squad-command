using Feature.GameplayECS.Common;
using Feature.GameplayECS.Movement;
using Feature.GameplayECS.View;
using Scellecs.Morpeh;
using UnityEngine;

namespace Feature.GameplayECS.Spawning.Systems
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
        
        public void OnAwake()
        {
            _unitTagStash = World.GetStash<UnitTag>();
            _movementSpeedStash = World.GetStash<MovementSpeed>();
            _assetPathStash  = World.GetStash<AssetPath>();
            _positionStash = World.GetStash<Position>();
            _rotationStash = World.GetStash<Rotation>();    
            _velocityStash = World.GetStash<Velocity>();
            
            var unit1 = World.CreateEntity();
            _unitTagStash.Add(unit1);
            _movementSpeedStash.Add(unit1, new MovementSpeed{Value = 3.5f});
            _assetPathStash.Add(unit1, new AssetPath{Value = "Unit"});
            _positionStash.Add(unit1, new Position {Value = Vector3.zero + Vector3.up * 1 + Vector3.right * 0});
            _velocityStash.Add(unit1, new Velocity {Value = Vector3.zero});
            _rotationStash.Add(unit1, new Rotation {Value = Quaternion.identity});
            
            var unit2 = World.CreateEntity();
            _unitTagStash.Add(unit2);
            _movementSpeedStash.Add(unit2, new MovementSpeed{Value = 3.5f});
            _assetPathStash.Add(unit2, new AssetPath{Value = "Unit"});
            _positionStash.Add(unit2, new Position {Value = Vector3.zero + Vector3.up * 1 + Vector3.right * 1});
            _velocityStash.Add(unit2, new Velocity {Value = Vector3.zero});
            _rotationStash.Add(unit2, new Rotation {Value = Quaternion.identity});
            
            var unit3 = World.CreateEntity();
            _unitTagStash.Add(unit3);
            _movementSpeedStash.Add(unit3, new MovementSpeed{Value = 3.5f});
            _assetPathStash.Add(unit3, new AssetPath{Value = "Unit"});
            _positionStash.Add(unit3, new Position {Value = Vector3.zero + Vector3.up * 1 + Vector3.right * -1});
            _velocityStash.Add(unit3, new Velocity {Value = Vector3.zero});
            _rotationStash.Add(unit3, new Rotation {Value = Quaternion.identity});
        }

        public void Dispose()
        {
            
        }
    }
}