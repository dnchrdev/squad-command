using System;
using Feature.GameplayECS.Infrastructure.UnitFactory;
using Feature.GameplayECS.Movement.Systems;
using Feature.GameplayECS.Navigation.Systems;
using Feature.GameplayECS.Select.Systems;
using Feature.GameplayECS.Spawning.Systems;
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
        
        public void Initialize()
        {
            var group = _world.CreateSystemsGroup();

            group.AddInitializer(new SpawnOnInitInitializer());
            
            group.AddSystem(new CreateViewSystem(_unitViewFactory));
            group.AddSystem(new CleanupViewSystem(_unitViewFactory));

            group.AddSystem(new SyncWithMotorSystem());
            
            group.AddSystem(new HideUnselectedSelfPreviewSystem());
            group.AddSystem(new ShowSelectedSelfPreviewSystem());
            group.AddSystem(new CommitSelectedSystem());

            group.AddSystem(new RecalculatePathSystem());
            group.AddSystem(new CalculateNavigationDirectionSystem());

            group.AddSystem(new ResetVelocitySystem());
            group.AddSystem(new ApplyMoveSpeedSystem());
            group.AddSystem(new ApplyVelocityToMotorSystem());
        
            _world.AddSystemsGroup(order: 0, group);
        }

        public void Dispose() => _world.Dispose();

    }
}