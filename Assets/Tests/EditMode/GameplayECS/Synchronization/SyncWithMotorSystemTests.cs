using System;
using Features.GameplayECS.Common.Components;
using Features.GameplayECS.Synchronization.Components;
using Features.GameplayECS.Synchronization.Systems;
using NUnit.Framework;
using Scellecs.Morpeh;
using Tests.EditMode.GameplayECS.Synchronization.Fakes;
using Tests.EditMode.TestUtils;
using UnityEngine;

namespace Tests.EditMode.GameplayECS.Synchronization
{
    public class SyncWithMotorSystemTests
    {
        private const float DT = 0.02f;

        private World _world;
        private SyncWithMotorSystem _system;

        [SetUp]
        public void SetUp()
        {
            _world = World.Create();
            _system = new SyncWithMotorSystem().Init(_world);
        }

        [TearDown]
        public void TearDown() => _world.Dispose();

        [Test]
        public void OnUpdate_CopiesMotorPositionAndRotation()
        {
            var motor = new FakeUnitMotor
            {
                Position = new Vector3(3f, 0f, 4f),
                Rotation = Quaternion.Euler(0f, 90f, 0f)
            };
            var unit = CreateUnit(motor);
            _world.Commit();

            _system.OnUpdate(DT);

            VectorAssert.AreEqual(motor.Position, _world.GetStash<Position>().Get(unit).Value);
            QuaternionAssert.AreEqual(motor.Rotation, _world.GetStash<Rotation>().Get(unit).Value);
        }

        [Test]
        public void OnUpdate_MissingMotor_Throws()
        {
            CreateUnit(null);
            _world.Commit();

            Assert.Throws<NullReferenceException>(() => _system.OnUpdate(DT));
        }

        private Entity CreateUnit(FakeUnitMotor motor)
        {
            var unit = _world.CreateEntity();
            _world.GetStash<Position>().Set(unit, new Position { Value = Vector3.zero });
            _world.GetStash<Rotation>().Set(unit, new Rotation { Value = Quaternion.identity });
            _world.GetStash<UnitMotorComponent>().Set(unit, new UnitMotorComponent { Value = motor });
            return unit;
        }
    }
}
