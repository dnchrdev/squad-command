using Features.GameplayECS.MoveCommand.Destination.Requests;
using Features.GameplayECS.MoveCommand.Preview.Requests;
using Features.Shared.Units;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.GameplayECS.Facade
{
    public sealed class MoveCommandsGateway : IMoveCommands
    {
        private readonly World _world;
        private readonly Stash<NewMovePreviewFormationRequest> _newMovePreviewRequestStash;
        private readonly Stash<UpdateMovePreviewFormationRequest> _updateMovePreviewRequestStash;
        private readonly Stash<HideMovePreviewSlotsRequest> _hideMovePreviewSlotsRequestStash;
        private readonly Stash<CommitMoveRequest> _commitMoveRequestStash;

        public MoveCommandsGateway(World world)
        {
            _world = world;
            _newMovePreviewRequestStash = world.GetStash<NewMovePreviewFormationRequest>();
            _updateMovePreviewRequestStash = world.GetStash<UpdateMovePreviewFormationRequest>();
            _hideMovePreviewSlotsRequestStash = world.GetStash<HideMovePreviewSlotsRequest>();
            _commitMoveRequestStash = world.GetStash<CommitMoveRequest>();
        }

        public void StartPreview()
        {
            _newMovePreviewRequestStash.Add(_world.CreateEntity(), new NewMovePreviewFormationRequest());
        }

        public void UpdatePreview(Vector2 center, Vector2 size, Vector2 formationForward, Vector2 formationRight)
        {
            _updateMovePreviewRequestStash.Add(_world.CreateEntity(), new UpdateMovePreviewFormationRequest
            {
                Center = center,
                Size = size,
                FormationForward = formationForward,
                FormationRight = formationRight
            });
        }

        public void CommitMove(Vector2 slotsOrderDirection)
        {
            _commitMoveRequestStash.Add(_world.CreateEntity(),
                new CommitMoveRequest { OrderDirection = slotsOrderDirection });
        }

        public void ClearPreview()
        {
            _hideMovePreviewSlotsRequestStash.Add(_world.CreateEntity());
        }
    }
}