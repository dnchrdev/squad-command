using Feature.GameplayECS.Common;
using Feature.GameplayECS.Movement;
using Scellecs.Morpeh;
using UnityEngine;

namespace Feature.GameplayECS.Physics.System
{
    // Разрешает коллизии/раздвижение юнитов. Идёт ПОСЛЕ SnapshotVelocityForPhysicsSystem,
    // ПЕРЕД ApplyVelocityToMotorSystem. PhysicsForce читается как чужая "желаемая скорость
    // после steering" (снимок, взятый до начала этого цикла) — так соседи узнают
    // скорость друг друга, а порядок перебора Filter не влияет на результат.
    // Читает Position (актуализируется другой системой), пишет итог обратно в Velocity.
    // Внимание: O(N^2), при большом кол-ве юнитов заменить на spatial hashing / grid.
    public class ResolveCollisionsSystem : IFixedSystem
    {
        private float _maxForce = 100f;

        public World World { get; set; }

        // Внешний цикл: только не-статичные тела (статики сами не двигаются)
        private Filter _movableBodiesFilter;

        // Внутренний цикл: все тела, с которыми возможна коллизия (статики + динамики)
        private Filter _allBodiesFilter;

        private Stash<Position> _positionStash;
        private Stash<CollisionRadius> _radiusStash;
        private Stash<StaticBodyTag> _staticStash;
        private Stash<PlayerControlledTag> _playerStash;
        private Stash<AIControlledTag> _aiStash;
        private Stash<PhysicsForce> _forceStash;
        private Stash<Velocity> _velocityStash;

        public void OnAwake()
        {
            _movableBodiesFilter = World.Filter
                .With<UnitTag>()
                .With<Position>()
                .With<CollisionRadius>()
                .With<PhysicsForce>()
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
            _forceStash = World.GetStash<PhysicsForce>();
            _velocityStash = World.GetStash<Velocity>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _movableBodiesFilter)
            {
                var objPos = _positionStash.Get(entity).Value;
                var objRadius = _radiusStash.Get(entity).Value;
                var objVelocity = _forceStash.Get(entity).Value;

                ref var velocity = ref _velocityStash.Get(entity);
                velocity.Value = objVelocity;

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

                    // "другая сторона" — принадлежит другой команде, чем obj
                    var isOtherSide = (objIsPlayer && otherIsAi) || (objIsAi && otherIsPlayer);

                    if (otherIsStatic || isOtherSide)
                    {
                        // Объект не толкаемый — скользим вдоль поверхности
                        var scalarProduct = Vector3.Dot(deltaDirection, -objVelocity.normalized);
                        var angle = Mathf.Acos(Mathf.Clamp(scalarProduct, -1f, 1f)) * Mathf.Rad2Deg;
                        var scale = Mathf.Cos(angle * Mathf.Deg2Rad);

                        if (scale > 0)
                        {
                            velocity.Value += deltaDirection * objVelocity.magnitude * scale;
                        }
                    }
                    else
                    {
                        // Столкновение со своим — толкаемся силой чужой скорости
                        var otherVelocity = _forceStash.Get(other).Value;
                        if (otherVelocity != Vector3.zero)
                        {
                            var otherSpeed = otherVelocity.magnitude;

                            var scalarProduct = Vector3.Dot(deltaDirection, otherVelocity.normalized);
                            var angle = Mathf.Acos(Mathf.Clamp(scalarProduct, -1f, 1f)) * Mathf.Rad2Deg;
                            var scale = Mathf.Cos(angle * Mathf.Deg2Rad);

                            if (scale > 0)
                            {
                                velocity.Value += deltaDirection * otherSpeed * scale;
                            }
                        }
                    }

                    // Анти-стек: если результирующая скорость нулевая, а объекты пересекаются — расталкиваем
                    if (velocity.Value == Vector3.zero)
                    {
                        if (currentDistance < 0.01f)
                        {
                            var randomVector = Random.onUnitSphere * _maxForce;
                            randomVector.y = 0;
                            velocity.Value += randomVector;
                        }
                        else if (currentDistance < minDistance)
                        {
                            velocity.Value += deltaDirection * Mathf.Min(minDistance / currentDistance, _maxForce);
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