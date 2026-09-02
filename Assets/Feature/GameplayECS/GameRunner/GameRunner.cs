using System;
using Feature.Core.Infrastructure;
using Feature.GameplayECS.MoveCommand;
using Feature.GameplayECS.MoveCommand.MovePreviewFactory;
using Feature.GameplayECS.MoveCommand.Systems;
using Feature.GameplayECS.Movement.Systems;
using Feature.GameplayECS.Navigation.Systems;
using Feature.GameplayECS.Physics.System;
using Feature.GameplayECS.SelectCommand.Systems;
using Feature.GameplayECS.Spawning.Systems;
using Feature.GameplayECS.Spawning.UnitFactory;
using Feature.GameplayECS.Synchronization.Systems;
using Feature.GameplayECS.View.Systems;
using Scellecs.Morpeh;
using Zenject;

namespace Feature.GameplayECS.GameRunner
{
    public sealed class GameRunner : IInitializable, IDisposable
    {
        [Inject] private readonly World _world;
        [Inject] private readonly IUnitViewFactory _unitViewFactory;
        [Inject] private readonly IMovePreviewFactory _movePreviewFactory;
        
        public void Initialize()
        {
            var group = _world.CreateSystemsGroup();

            group.AddInitializer(new SpawnOnInitInitializer());
            
            group.AddSystem(new CreateViewSystem(_unitViewFactory));
            group.AddSystem(new CleanupViewSystem(_unitViewFactory));

            group.AddSystem(new SyncWithMotorSystem());
            
            group.AddSystem(new HideSelectPreviewSystem());
            group.AddSystem(new ShowSelectPreviewSystem());
            group.AddSystem(new CommitSelectedSystem());

            var previewPool = new ObjectPool<MovePreviewView>();
            
            
            group.AddSystem(new MovePreviewHideSystem(previewPool));
            group.AddSystem(new MovePreviewShowSystem(previewPool));
            group.AddSystem(new MovePreviewCreateSystem(previewPool, _movePreviewFactory));
            group.AddSystem(new MovePreviewSlotRepositionSystem());
            group.AddSystem(new CommitMoveSystem());
            
            //group.AddSystem(new SquadFormationSystem());
            group.AddSystem(new ApplyDestinationRequest());
            group.AddSystem(new CalculateNavigationDirectionSystem());

            group.AddSystem(new ResetVelocitySystem());
            group.AddSystem(new ApplyMoveSpeedSystem());
            group.AddSystem(new SnapshotVelocityForPhysicsSystem());
            group.AddSystem(new ResolveCollisionsSystem());
            group.AddSystem(new ApplyVelocityToMotorSystem());
        
            _world.AddSystemsGroup(order: 0, group);
        }

        public void Dispose() => _world.Dispose();

    }
}