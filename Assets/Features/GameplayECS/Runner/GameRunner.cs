using System;
using Features.GameplayECS.MoveCommand;
using Features.GameplayECS.MoveCommand.Ports;
using Features.GameplayECS.Movement;
using Features.GameplayECS.Navigation;
using Features.GameplayECS.Navigation.Systems.Ports;
using Features.GameplayECS.Physics;
using Features.GameplayECS.Physics.Systems.Ports;
using Features.GameplayECS.Selection;
using Features.GameplayECS.Spawning;
using Features.GameplayECS.Synchronization;
using Features.GameplayECS.UnitView;
using Features.GameplayECS.UnitView.Systems.Ports;
using Scellecs.Morpeh;
using Zenject;

namespace Features.GameplayECS.Runner
{
    public sealed class GameRunner : IInitializable, IDisposable
    {
        private readonly World _world;
        private readonly IUnitViewFactory _unitViewFactory;
        private readonly IMoveSlotViewFactory _moveSlotViewFactory;
        private readonly IPathService _pathService;
        private readonly IRandomSource _randomSource;

        public GameRunner(
            World world,
            IUnitViewFactory unitViewFactory,
            IMoveSlotViewFactory moveSlotViewFactory,
            IPathService pathService,
            IRandomSource randomSource)
        {
            _world = world;
            _unitViewFactory = unitViewFactory;
            _moveSlotViewFactory = moveSlotViewFactory;
            _pathService = pathService;
            _randomSource = randomSource;
        }

        public void Initialize()
        {
            var group = _world.CreateSystemsGroup();

            SpawningFeature.Add(group);
            UnitViewFeature.Add(group, _unitViewFactory);
            SynchronizationFeature.AddReadFromUnity(group);
            SelectionFeature.Add(group);
            MoveCommandFeature.Add(group, _moveSlotViewFactory);
            NavigationFeature.Add(group, _pathService);
            MovementFeature.Add(group);
            PhysicsFeature.Add(group, _randomSource);
            SynchronizationFeature.AddWriteToUnity(group);

            _world.AddSystemsGroup(order: 0, group);
        }

        public void Dispose() => _world.Dispose();
    }
}