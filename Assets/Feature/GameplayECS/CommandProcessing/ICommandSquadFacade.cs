using System.Collections.Generic;
using Feature.GameplayECS.Common;
using Scellecs.Morpeh;
using UnityEngine;

namespace Feature.GameplayECS.CommandProcessing
{
    public interface ICommandSquadFacade
    {
        void SelectPreview(IReadOnlyList<Entity> entities);
        void ToggleSelectPreview(IReadOnlyList<Entity> entities);
        void UnselectPreview(IReadOnlyList<Entity> entities);
        void ClearSelection();
        void MoveSelectedTo(Vector3  targetPosition);
        void CommitSelected();
    }
}