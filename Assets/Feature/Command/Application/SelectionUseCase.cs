using System.Collections.Generic;
using Feature.GameplayECS.CommandProcessing;
using Scellecs.Morpeh;
using Zenject;

namespace Feature.Command.Application
{
    public class SelectionUseCase
    {
        [Inject] private readonly ICommandSquadFacade _commandFacade;
        [Inject] private readonly IUnitQueryFacade _queryFacade;

        public void SelectSingle(Entity entity)
        {
            _commandFacade.ToggleSelectPreview(new[] { entity });
        }

        public void SelectMany(IReadOnlyList<Entity> entities)
        {
            _commandFacade.SelectPreview(entities);
        }

        public void Clear()
        {
            _commandFacade.ClearSelection();
        }
        
        public void CommitSelected()
        {
            _commandFacade.CommitSelected();
        }

        public Filter GetPreviewSelectedFilter()
        {
            return _queryFacade.PreviewSelectedUnits;
        }
    }
}