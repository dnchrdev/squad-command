using Features.GameplayECS.Common.View;
using Features.GameplayECS.UnitView.Components;
using Features.GameplayECS.UnitView.Requests;
using Features.GameplayECS.UnitView.Systems;
using NUnit.Framework;
using Scellecs.Morpeh;
using Tests.EditMode.GameplayECS.UnitView.Fakes;
using Tests.EditMode.TestUtils;
using UnityEngine;

namespace Tests.EditMode.GameplayECS.UnitView
{
    public class CleanupViewSystemTests
    {
        private const float DT = 0.016f;

        private World _world;
        private StubUnitViewFactory _factory;
        private CleanupViewSystem _system;
        private MonoEntity _view;
        private Entity _unit;

        [SetUp]
        public void SetUp()
        {
            _world = World.Create();
            _factory = new StubUnitViewFactory();
            _system = new CleanupViewSystem(_factory).Init(_world);

            _view = new GameObject(nameof(MonoEntity)).AddComponent<MonoEntity>();
            _unit = _world.CreateEntity();
            _view.Bind(_unit, _world);
            _world.GetStash<UnitViewComponent>().Set(_unit, new UnitViewComponent { Value = _view });
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_view.gameObject);
            _world.Dispose();
        }

        [Test]
        public void OnUpdate_DestroyRequest_UnbindsAndReleasesView()
        {
            _world.GetStash<DestroySelfRequest>().Set(_unit);
            _world.Commit();

            _system.OnUpdate(DT);

            Assert.That(_factory.Released, Is.EqualTo(new[] { _view }));
            Assert.That(_view.World, Is.Null);
        }

        [Test]
        public void OnUpdate_NoRequest_KeepsView()
        {
            _world.Commit();

            _system.OnUpdate(DT);

            Assert.That(_factory.Released, Is.Empty);
            Assert.That(_view.World, Is.SameAs(_world));
        }

        [Test]
        public void Dispose_ReleasesAllViews()
        {
            _world.Commit();

            _system.Dispose();

            Assert.That(_factory.Released, Is.EqualTo(new[] { _view }));
        }
    }
}
