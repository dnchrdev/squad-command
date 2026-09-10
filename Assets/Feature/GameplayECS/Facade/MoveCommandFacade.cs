using Feature.GameplayECS.Facade.Interfaces;
using Feature.GameplayECS.MoveCommand;
using Feature.GameplayECS.MoveCommand.Systems.Destination;
using Feature.GameplayECS.MoveCommand.Systems.Preview;
using Scellecs.Morpeh;
using UnityEngine;

namespace Feature.GameplayECS.Facade
{
    public class MoveCommandFacade : IMoveCommandFacade
    {
        private readonly World _world;
        private readonly Stash<NewMovePreviewFormationRequest> _newMovePreviewRequestStash;
        private readonly Stash<UpdateMovePreviewFormationRequest> _updateMovePreviewRequestStash;
        private readonly Stash<HideMovePreviewSlotsRequest> _hideMovePreviewSlotsRequestStash;
        private readonly Stash<CommitMoveRequest> _commitMoveRequestStash;

        public MoveCommandFacade(World world)
        {
            _world = world;

            _newMovePreviewRequestStash = world.GetStash<NewMovePreviewFormationRequest>();
            _updateMovePreviewRequestStash = world.GetStash<UpdateMovePreviewFormationRequest>();
            _hideMovePreviewSlotsRequestStash = world.GetStash<HideMovePreviewSlotsRequest>();
            _commitMoveRequestStash = world.GetStash<CommitMoveRequest>();
        }


        public void NewMovePreviewRequest()
        {
            _newMovePreviewRequestStash.Add(_world.CreateEntity(), new NewMovePreviewFormationRequest());
        }

        public void UpdateMovePreviewRequest(Vector2 center, Vector2 size, Vector2 formationForward,
            Vector2 formationRight)
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

        public void ClearPreviewRequest()
        {
            _hideMovePreviewSlotsRequestStash.Add(_world.CreateEntity());
        }
    }
}