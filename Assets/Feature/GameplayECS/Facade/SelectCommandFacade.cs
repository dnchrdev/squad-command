using System.Collections.Generic;
using Feature.GameplayECS.Common;
using Feature.GameplayECS.Facade.Interfaces;
using Feature.GameplayECS.SelectCommand;
using Scellecs.Morpeh;

namespace Feature.GameplayECS.Facade
{
    public sealed class SelectCommandFacade : ISelectCommandFacade
    {
        private readonly World _world;
        private readonly Filter _selectedFilter;
        private readonly Filter _previewSelectedFilter;
        private readonly Filter _previewOnlyFilter;
        private readonly Filter _previewUnselectedFilter;
        private readonly Stash<Selected> _selectedStash;
        private readonly Stash<SelectSelfRequest> _selectSelfStash;
        private readonly Stash<UnselectSelfRequest> _unselectSelfStash;
        private readonly Stash<CommitSelectedRequest> _commitSelectedRequestStash;

        public SelectCommandFacade(World world)
        {
            _world = world;
            _selectedFilter = world.Filter.With<UnitTag>().With<Selected>().Build();
            _previewSelectedFilter = world.Filter.With<UnitTag>().With<SelectSelfRequest>().Build();
            _previewOnlyFilter = world.Filter.With<UnitTag>().With<SelectSelfRequest>().Without<Selected>().Build();
            _previewUnselectedFilter = world.Filter.With<UnitTag>().With<UnselectSelfRequest>().Build();
            _selectedStash = world.GetStash<Selected>();
            _selectSelfStash = world.GetStash<SelectSelfRequest>();
            _unselectSelfStash = world.GetStash<UnselectSelfRequest>();
            _commitSelectedRequestStash = world.GetStash<CommitSelectedRequest>();
        }

        public void SelectPreview(IReadOnlyList<Entity> entities)
        {
            ClearPreview();
            MarkPreview(entities);
        }

        public void SelectPreviewWithClear(IReadOnlyList<Entity> entities)
        {
            ClearPreview();
            ClearSelected();
            MarkPreview(entities);
        }

        public void ToggleSelectPreview(IReadOnlyList<Entity> entities)
        {
            ClearPreview();
            for (int i = 0; i < entities.Count; i++)
            {
                var entity = entities[i];
                if (_world.IsDisposed(entity)) continue;
                if (_selectedStash.Has(entity))
                {
                    _selectedStash.Remove(entity);
                    _selectSelfStash.Remove(entity);
                }
                else
                {
                    _selectSelfStash.Set(entity);
                }
            }
        }

        public void UnselectPreview(IReadOnlyList<Entity> entities)
        {
            ClearPreviewUnselection();
            for (int i = 0; i < entities.Count; i++)
            {
                var entity = entities[i];
                if (_world.IsDisposed(entity)) continue;
                _unselectSelfStash.Set(entity);
            }
        }

        public void ClearAllSelected()
        {
            ClearSelected();
            ClearPreviewSelection();
        }

        public void ClearAllSelectRequests()
        {
            ClearPreviewSelection();
            ClearPreviewUnselection();
        }

        public void CommitSelected()
        {
            _commitSelectedRequestStash.Add(_world.CreateEntity());
        }

        private void MarkPreview(IReadOnlyList<Entity> entities)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                var entity = entities[i];
                if (!_world.IsDisposed(entity))
                    _selectSelfStash.Set(entity);
            }
        }

        private void ClearSelected()
        {
            foreach (var entity in _selectedFilter)
                _selectedStash.Remove(entity);
        }

        private void ClearPreview()
        {
            foreach (var entity in _previewOnlyFilter)
                _selectSelfStash.Remove(entity);
        }

        private void ClearPreviewSelection()
        {
            foreach (var entity in _previewSelectedFilter)
                _selectSelfStash.Remove(entity);
        }

        private void ClearPreviewUnselection()
        {
            foreach (var entity in _previewUnselectedFilter)
                _unselectSelfStash.Remove(entity);
        }
    }
}