using System.Collections.Generic;
using Feature.GameplayECS.Common;
using Feature.GameplayECS.Movement;
using Feature.GameplayECS.Navigation;
using Feature.GameplayECS.Select;
using Scellecs.Morpeh;
using UnityEngine;

namespace Feature.GameplayECS.CommandProcessing
{
    public sealed class CommandSquadFacade : ICommandSquadFacade
    {
        private readonly World _world;
        
        private readonly Filter _selectedFilter;
        private readonly Filter _selectedSelfFilter;
        private readonly Filter _selectedPreviewfFilter;

        private readonly Stash<Selected> _selectedStash;
        private readonly Stash<SelectSelfRequest> _selectSelfStash;
        private readonly Stash<UnselectSelfRequest> _unselectSelfStash;
        private readonly Stash<Destination> _destinationStash;
        
        private readonly Stash<RecalculateSelfPathRequest> _recalculateSelfPathRequestStash;
        private readonly Stash<CommitSelectedRequest>  _commitSelectedStash;

        public CommandSquadFacade(World world)
        {
            _world = world;
            
            _selectedFilter = _world.Filter
                .With<UnitTag>()
                .With<Selected>().Build();
            
            _selectedSelfFilter = _world.Filter
                .With<UnitTag>()
                .With<SelectSelfRequest>()
                .Build();
            
            _selectedPreviewfFilter = _world.Filter
                .With<UnitTag>()
                .With<SelectSelfRequest>()
                .Without<Selected>()
                .Build();

            _selectedStash = _world.GetStash<Selected>();
            _selectSelfStash = _world.GetStash<SelectSelfRequest>();
            _unselectSelfStash = _world.GetStash<UnselectSelfRequest>();
            _destinationStash = _world.GetStash<Destination>();
            _recalculateSelfPathRequestStash = _world.GetStash<RecalculateSelfPathRequest>();
            _commitSelectedStash = _world.GetStash<CommitSelectedRequest>();   
        }

        public void SelectPreview(IReadOnlyList<Entity> entities)
        {
            ClearSelectionPreviewInternal();
            
            foreach (var entity in entities)
            {
                if (_world.IsDisposed(entity)) continue;

                _selectSelfStash.Set(entity);
            }
        }
        
        public void ToggleSelectPreview(IReadOnlyList<Entity> entities)
        {
            ClearSelectionPreviewInternal();
            
            foreach (var entity in entities)
            {
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
            foreach (var entity in entities)
            {
                if (_world.IsDisposed(entity)) continue;

                _unselectSelfStash.Set(entity);
            }
        }

        public void ClearSelection()
        { 
            ClearSelectionInternal();
            ClearSelectionSelfInternal();
        }

        public void MoveSelectedTo(Vector3 targetPosition)
        {
            foreach (var entity in _selectedFilter)
            {
                if (_world.IsDisposed(entity)) continue;
                
                _destinationStash.Set(entity, new Destination{Target = targetPosition});
                _recalculateSelfPathRequestStash.Set(entity);
            }
        }

        public void CommitSelected()
        {
            var commitRequest = _world.CreateEntity();
            _commitSelectedStash.Add(commitRequest);
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
            foreach (var entity in _selectedPreviewfFilter)
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
        
        
    }
}