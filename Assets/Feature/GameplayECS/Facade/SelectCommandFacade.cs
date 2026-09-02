using System.Collections.Generic;
using Feature.GameplayECS.Common;
using Feature.GameplayECS.Facade.Interfaces;
using Feature.GameplayECS.Navigation;
using Feature.GameplayECS.SelectCommand;
using Scellecs.Morpeh;
using UnityEngine;

namespace Feature.GameplayECS.Facade
{
    public sealed class SelectCommandFacade : ISelectCommandFacade
    {
        private readonly World _world;
        
        private readonly Filter _selectedFilter;
        private readonly Filter _selectedSelfFilter;
        private readonly Filter _unselectedSelfFilter;
        private readonly Filter _selfSelectedOnlyPreviewFilter;

        private readonly Stash<Selected> _selectedStash;
        private readonly Stash<SelectSelfRequest> _selectSelfStash;
        private readonly Stash<UnselectSelfRequest> _unselectSelfStash;
        
        private readonly Stash<CommitSelectedRequest>  _commitSelectedRequestStash;

        public SelectCommandFacade(World world)
        {
            _world = world;
            
            _selectedFilter = _world.Filter
                .With<UnitTag>()
                .With<Selected>().Build();
            
            _selectedSelfFilter = _world.Filter
                .With<UnitTag>()
                .With<SelectSelfRequest>()
                .Build();
            
            _selfSelectedOnlyPreviewFilter = _world.Filter
                .With<UnitTag>()
                .With<SelectSelfRequest>()
                .Without<Selected>()
                .Build();
            
            _unselectedSelfFilter = _world.Filter
                .With<UnitTag>()
                .With<UnselectSelfRequest>()
                .Build();

            _selectedStash = _world.GetStash<Selected>();
            _selectSelfStash = _world.GetStash<SelectSelfRequest>();
            _unselectSelfStash = _world.GetStash<UnselectSelfRequest>();
            _commitSelectedRequestStash = _world.GetStash<CommitSelectedRequest>();   
        }

        public void SelectPreview(IReadOnlyList<Entity> entities)
        {
            ClearSelectionPreviewInternal();
            for (int i = 0; i < entities.Count; i++)
            {
                var entity = entities[i];
                if (_world.IsDisposed(entity)) continue;
                _selectSelfStash.Set(entity);
            }
        }

        public void SelectPreviewWithClear(IReadOnlyList<Entity> entities)
        {
            ClearSelectionPreviewInternal();
            ClearSelectionInternal();
            for (int i = 0; i < entities.Count; i++)
            {
                var entity = entities[i];
                if (_world.IsDisposed(entity)) continue;
                _selectSelfStash.Set(entity);
            }
        }

        public void ToggleSelectPreview(IReadOnlyList<Entity> entities)
        {
            ClearSelectionPreviewInternal();
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
            ClearUnselectionSelfInternal();
            for (int i = 0; i < entities.Count; i++)
            {
                var entity = entities[i];
                if (_world.IsDisposed(entity)) continue;
                _unselectSelfStash.Set(entity);
            }
        }

        public void ClearAllSelected()
        { 
            ClearSelectionInternal();
            ClearSelectionSelfInternal();
        }
        
        public void ClearAllSelectRequests()
        { 
            ClearSelectionSelfInternal();
            ClearUnselectionSelfInternal();
        }

        public void CommitSelected()
        {
            var commitRequest = _world.CreateEntity();
            _commitSelectedRequestStash.Add(commitRequest);
        }

        private void ClearSelectionInternal()
        {
            foreach (var entity in _selectedFilter)
            {
                _selectedStash.Remove(entity);
            }
        }
        private void ClearSelectionPreviewInternal()
        {
            foreach (var entity in _selfSelectedOnlyPreviewFilter)
            {
                _selectSelfStash.Remove(entity);
            }
        }
        
        private void ClearSelectionSelfInternal()
        {
            foreach (var entity in _selectedSelfFilter)
            {
                _selectSelfStash.Remove(entity);
            }
        }
        
        private void ClearUnselectionSelfInternal()
        {
            foreach (var entity in _unselectedSelfFilter)
            {
                _unselectSelfStash.Remove(entity);
            }
        }
        
    }
}