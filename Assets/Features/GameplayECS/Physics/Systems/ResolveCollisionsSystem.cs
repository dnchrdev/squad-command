using Features.GameplayECS.Common.Components;
using Features.GameplayECS.Movement.Outputs;
using Features.GameplayECS.Physics.Components;
using Features.GameplayECS.Physics.Outputs;
using Features.GameplayECS.Physics.Systems.Ports;
using Features.GameplayECS.Synchronization.Outputs;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.GameplayECS.Physics.Systems
{
    public class ResolveCollisionsSystem : IFixedSystem
    {
        private const float MAX_FORCE = 100f;

        private readonly IRandomSource _random;

        public World World { get; set; }

        public ResolveCollisionsSystem(IRandomSource random)
        {
            _random = random;
        }

        private Filter _movableBodiesFilter;

        private Filter _allBodiesFilter;

        private Stash<Position> _positionStash;
        private Stash<CollisionRadius> _radiusStash;
        private Stash<StaticBodyTag> _staticStash;
        private Stash<PlayerControlledTag> _playerStash;
        private Stash<AIControlledTag> _aiStash;
        private Stash<DesiredVelocity> _desiredVelocityStash;
        private Stash<Velocity> _velocityStash;

        public void OnAwake()
        {
            _movableBodiesFilter = World.Filter
                .With<UnitTag>()
                .With<Position>()
                .With<CollisionRadius>()
                .With<DesiredVelocity>()
                .With<Velocity>()
                .Without<StaticBodyTag>()
                .Build();

            _allBodiesFilter = World.Filter
                .With<UnitTag>()
                .With<Position>()
                .With<CollisionRadius>()
                .Build();

            _positionStash = World.GetStash<Position>();
            _radiusStash = World.GetStash<CollisionRadius>();
            _staticStash = World.GetStash<StaticBodyTag>();
            _playerStash = World.GetStash<PlayerControlledTag>();
            _aiStash = World.GetStash<AIControlledTag>();
            _desiredVelocityStash = World.GetStash<DesiredVelocity>();
            _velocityStash = World.GetStash<Velocity>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _movableBodiesFilter)
            {
                var objPos = _positionStash.Get(entity).Value;
                var objRadius = _radiusStash.Get(entity).Value;
                var objDesiredVelocity = _desiredVelocityStash.Get(entity).Value;

                ref var velocity = ref _velocityStash.Get(entity);
                velocity.Value = objDesiredVelocity;

                var objIsPlayer = _playerStash.Has(entity);
                var objIsAi = _aiStash.Has(entity);

                foreach (var other in _allBodiesFilter)
                {
                    if (other == entity)
                    {
                        continue;
                    }

                    var otherPos = _positionStash.Get(other).Value;
                    var otherRadius = _radiusStash.Get(other).Value;

                    var deltaVector = objPos - otherPos;
                    var deltaDirection = deltaVector.normalized;

                    var currentDistance = deltaVector.magnitude;
                    var minDistance = objRadius + otherRadius;

                    if (currentDistance > minDistance)
                    {
                        continue;
                    }

                    var otherIsStatic = _staticStash.Has(other);
                    var otherIsPlayer = _playerStash.Has(other);
                    var otherIsAi = _aiStash.Has(other);

                    var isOtherSide = (objIsPlayer && otherIsAi) || (objIsAi && otherIsPlayer);

                    if (otherIsStatic || isOtherSide)
                    {
                        var scalarProduct = Vector3.Dot(deltaDirection, -objDesiredVelocity.normalized);
                        var angle = Mathf.Acos(Mathf.Clamp(scalarProduct, -1f, 1f)) * Mathf.Rad2Deg;
                        var scale = Mathf.Cos(angle * Mathf.Deg2Rad);

                        if (scale > 0)
                        {
                            velocity.Value += deltaDirection * objDesiredVelocity.magnitude * scale;
                        }
                    }
                    else
                    {
                        var otherDesiredVelocity = _desiredVelocityStash.Get(other).Value;
                        if (otherDesiredVelocity != Vector3.zero)
                        {
                            var otherSpeed = otherDesiredVelocity.magnitude;

                            var scalarProduct = Vector3.Dot(deltaDirection, otherDesiredVelocity.normalized);
                            var angle = Mathf.Acos(Mathf.Clamp(scalarProduct, -1f, 1f)) * Mathf.Rad2Deg;
                            var scale = Mathf.Cos(angle * Mathf.Deg2Rad);

                            if (scale > 0)
                            {
                                velocity.Value += deltaDirection * otherSpeed * scale;
                            }
                        }
                    }

                    if (velocity.Value == Vector3.zero)
                    {
                        if (currentDistance < 0.01f)
                        {
                            var randomVector = _random.OnUnitSphere() * MAX_FORCE;
                            randomVector.y = 0;
                            velocity.Value += randomVector;
                        }
                        else if (currentDistance < minDistance)
                        {
                            velocity.Value += deltaDirection * Mathf.Min(minDistance / currentDistance, MAX_FORCE);
                        }
                    }
                }
            }
        }

        public void Dispose()
        {
        }
    }
}