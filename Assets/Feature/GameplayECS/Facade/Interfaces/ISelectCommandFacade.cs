using System.Collections.Generic;
using Scellecs.Morpeh;

namespace Feature.GameplayECS.Facade.Interfaces
{
    public interface ISelectCommandFacade
    {
        void SelectPreview(IReadOnlyList<Entity> entities);
        void SelectPreviewWithClear(IReadOnlyList<Entity> entities);
        void ToggleSelectPreview(IReadOnlyList<Entity> entities);
        void UnselectPreview(IReadOnlyList<Entity> entities);
        void ClearAllSelected();
        void ClearAllSelectRequests();
        void CommitSelected();
    }
}