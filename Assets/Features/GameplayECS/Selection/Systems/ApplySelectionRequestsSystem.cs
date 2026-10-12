using System;
using System.Collections.Generic;
using Features.GameplayECS.Common;
using Features.GameplayECS.Common.Components;
using Features.GameplayECS.Selection.Components;
using Features.GameplayECS.Selection.Outputs;
using Features.GameplayECS.Selection.Requests;
using Scellecs.Morpeh;

namespace Features.GameplayECS.Selection.Systems
{
    public sealed class ApplySelectionRequestsSystem : ISystem
    {
        private static readonly Comparison<SelectionRequest> BySequence =
            (a, b) => a.Sequence.CompareTo(b.Sequence);

        public World World { get; set; }

        private Filter _requestFilter;
        private Filter _unitsFilter;

        private Stash<SelectionRequest> _requestStash;
        private Stash<SelectedTag> _selectedStash;
        private Stash<SelectPreviewTag> _selectPreviewStash;
        private Stash<UnselectPreviewTag> _unselectPreviewStash;
        private Stash<SelectViewComponent> _selectViewStash;
        private Stash<SelectedViewShowedTag> _selectedShowedStash;

        private readonly List<SelectionRequest> _requestsBuffer = new();

        public void OnAwake()
        {
            _requestFilter = World.Filter.With<SelectionRequest>().Build();
            _unitsFilter = World.Filter.With<UnitTag>().Build();

            _requestStash = World.GetStash<SelectionRequest>();
            _selectedStash = World.GetStash<SelectedTag>();
            _selectPreviewStash = World.GetStash<SelectPreviewTag>();
            _unselectPreviewStash = World.GetStash<UnselectPreviewTag>();
            _selectViewStash = World.GetStash<SelectViewComponent>();
            _selectedShowedStash = World.GetStash<SelectedViewShowedTag>();
        }

        public void OnUpdate(float deltaTime)
        {
            _requestsBuffer.Clear();
            foreach (var entity in _requestFilter)
                _requestsBuffer.Add(_requestStash.Get(entity));

            if (_requestsBuffer.Count == 0) return;

            _requestsBuffer.Sort(BySequence);

            for (int i = 0; i < _requestsBuffer.Count; i++)
                Apply(_requestsBuffer[i]);

            RequestUtils.ConsumeAllRequests(World, _requestFilter, _requestStash);
        }

        private void Apply(SelectionRequest request)
        {
            switch (request.Type)
            {
                case SelectionRequestType.PreviewAdd:
                    ClearPreviewOnly();
                    MarkSelectPreview(request.Units);
                    break;
                case SelectionRequestType.PreviewReplace:
                    ClearPreviewOnly();
                    ClearSelected();
                    MarkSelectPreview(request.Units);
                    break;
                case SelectionRequestType.PreviewRemove:
                    ClearUnselectPreview();
                    MarkUnselectPreview(request.Units);
                    break;
                case SelectionRequestType.ClearSelection:
                    ClearSelected();
                    ClearSelectPreview();
                    break;
                case SelectionRequestType.ClearPreview:
                    ClearSelectPreview();
                    ClearUnselectPreview();
                    break;
                case SelectionRequestType.Commit:
                    Commit();
                    break;
            }
        }

        private void MarkSelectPreview(List<Entity> units)
        {
            for (int i = 0; i < units.Count; i++)
            {
                if (World.IsDisposed(units[i])) continue;
                _selectPreviewStash.Set(units[i]);
            }
        }

        private void MarkUnselectPreview(List<Entity> units)
        {
            for (int i = 0; i < units.Count; i++)
            {
                if (World.IsDisposed(units[i])) continue;
                _unselectPreviewStash.Set(units[i]);
            }
        }

        private void ClearPreviewOnly()
        {
            foreach (var unit in _unitsFilter)
            {
                if (_selectPreviewStash.Has(unit) && !_selectedStash.Has(unit))
                    _selectPreviewStash.Remove(unit);
            }
        }

        private void ClearSelected()
        {
            foreach (var unit in _unitsFilter)
            {
                if (_selectedStash.Has(unit))
                    _selectedStash.Remove(unit);
            }
        }

        private void ClearSelectPreview()
        {
            foreach (var unit in _unitsFilter)
            {
                if (_selectPreviewStash.Has(unit))
                    _selectPreviewStash.Remove(unit);
            }
        }

        private void ClearUnselectPreview()
        {
            foreach (var unit in _unitsFilter)
            {
                if (_unselectPreviewStash.Has(unit))
                    _unselectPreviewStash.Remove(unit);
            }
        }

        private void Commit()
        {
            foreach (var unit in _unitsFilter)
            {
                if (!_selectViewStash.Has(unit)) continue;

                if (_selectPreviewStash.Has(unit) && !_selectedStash.Has(unit))
                {
                    ShowSelectedView(unit);
                    _selectPreviewStash.Remove(unit);
                    _selectedStash.Set(unit);
                }

                if (_unselectPreviewStash.Has(unit))
                {
                    HideSelectedView(unit);
                    _selectedStash.Remove(unit);
                    _unselectPreviewStash.Remove(unit);
                }
            }
        }

        private void ShowSelectedView(Entity unit)
        {
            if (_selectedShowedStash.Has(unit)) return;
            _selectViewStash.Get(unit).Value.Show();
            _selectedShowedStash.Add(unit);
        }

        private void HideSelectedView(Entity unit)
        {
            if (!_selectedShowedStash.Has(unit)) return;
            _selectViewStash.Get(unit).Value.Hide();
            _selectedShowedStash.Remove(unit);
        }

        public void Dispose()
        {
        }
    }
}