using Feature.GameplayECS.Facade.Interfaces;
using Feature.GameplayECS.MoveCommand;
using Scellecs.Morpeh;
using UnityEngine;

namespace Feature.GameplayECS.Facade
{
    public class MoveCommandFacade : IMoveCommandFacade
    {
        private readonly World _world;
        private readonly Stash<MovePreviewRequest> _movePreviewRequestStash;
        private readonly Stash<HideMovePreviewRequest> _hideMovePreviewRequestStash;
        private readonly Stash<CommitMoveRequest> _commitMoveRequestStash;

        public MoveCommandFacade(World world)
        {
            _world = world;
            _movePreviewRequestStash = world.GetStash<MovePreviewRequest>();
            _hideMovePreviewRequestStash = world.GetStash<HideMovePreviewRequest>();
            _commitMoveRequestStash = world.GetStash<CommitMoveRequest>();
        }

        public void ShowAllMoveDestinations()
        {
        }

        public void StopAll()
        {
            HideMovePreview();
        }

        public void ClearSelected()
        {
        }

        public void MovePreviewRequest(Vector2 center, Vector2 size, Vector2 formationForward, Vector2 formationRight)
        {
            var movePreviewRequest = _world.CreateEntity();

            _movePreviewRequestStash.Add(movePreviewRequest, new MovePreviewRequest
            {
                Center = center,
                Size = size,
                FormationForward = formationForward,
                FormationRight = formationRight
            });
        }

        public void CommitMove(Vector2 slotsOrderDirection)
        {
            var commitEntity = _world.CreateEntity();
            _commitMoveRequestStash.Add(commitEntity, new CommitMoveRequest { FormationForward = slotsOrderDirection});
        }

        private void HideMovePreview()
        {
            var hideRequest = _world.CreateEntity();
            _hideMovePreviewRequestStash.Add(hideRequest, new HideMovePreviewRequest());
        }
    }
}