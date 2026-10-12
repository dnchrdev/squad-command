using Features.GameplayECS.Common.Components;
using Features.GameplayECS.Common.View;
using Features.GameplayECS.UnitView.Components;
using Features.GameplayECS.UnitView.Systems;
using NUnit.Framework;
using Scellecs.Morpeh;
using Tests.EditMode.GameplayECS.UnitView.Fakes;
using Tests.EditMode.TestUtils;
using UnityEngine;

namespace Tests.EditMode.GameplayECS.UnitView
{
    public class CreateViewSystemTests
    {
        private const float DT = 0.016f;
        private const string ASSET_PATH = "Unit";

        private static readonly Vector3 SpawnPosition = new Vector3(1f, 0f, 2f);
        private static readonly Quaternion SpawnRotation = Quaternion.Euler(0f, 90f, 0f);

        private World _world;
        private StubUnitViewFactory _factory;
        private CreateViewSystem _system;
        private MonoEntity _view;

        [SetUp]
        public void SetUp()
        {
            _world = World.Create();
            _view = new GameObject(nameof(MonoEntity)).AddComponent<MonoEntity>();
            _factory = new StubUnitViewFactory { NextView = _view };
            _system = new CreateViewSystem(_factory).Init(_world);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_view.gameObject);
            _world.Dispose();
        }

        [Test]
        public void OnUpdate_RequestsViewWithUnitPlacement()
        {
            CreateUnit();
            _world.Commit();

            _system.OnUpdate(DT);

            Assert.That(_factory.RequestedPaths, Is.EqualTo(new[] { ASSET_PATH }));
            VectorAssert.AreEqual(SpawnPosition, _factory.LastPosition);
            QuaternionAssert.AreEqual(SpawnRotation, _factory.LastRotation);
        }

        [Test]
        public void OnUpdate_ViewLoaded_AttachesBoundView()
        {
            var unit = CreateUnit();
            _world.Commit();

            _system.OnUpdate(DT);

            Assert.That(_world.GetStash<UnitViewComponent>().Get(unit).Value, Is.SameAs(_view));
            Assert.That(_view.Entity, Is.EqualTo(unit));
            Assert.That(_world.GetStash<SpawningTag>().Has(unit), Is.False);
        }

        [Test]
        public void OnUpdate_WhileLoading_MarksSpawningAndRequestsOnce()
        {
            _factory.Pending = true;
            var unit = CreateUnit();
            _world.Commit();

            _system.OnUpdate(DT);
            _world.Commit();
            _system.OnUpdate(DT);

            Assert.That(_factory.RequestedPaths, Has.Count.EqualTo(1));
            Assert.That(_world.GetStash<SpawningTag>().Has(unit), Is.True);
            Assert.That(_world.GetStash<UnitViewComponent>().Has(unit), Is.False);
        }

        [Test]
        public void OnUpdate_UnitRemovedWhileLoading_ReleasesView()
        {
            _factory.Pending = true;
            var unit = CreateUnit();
            _world.Commit();
            _system.OnUpdate(DT);

            _world.RemoveEntity(unit);
            _world.Commit();
            _factory.CompletePending();

            Assert.That(_factory.Released, Is.EqualTo(new[] { _view }));
        }

        private Entity CreateUnit()
        {
            var unit = _world.CreateEntity();
            _world.GetStash<AssetPath>().Set(unit, new AssetPath { Value = ASSET_PATH });
            _world.GetStash<Position>().Set(unit, new Position { Value = SpawnPosition });
            _world.GetStash<Rotation>().Set(unit, new Rotation { Value = SpawnRotation });
            return unit;
        }
    }
}
