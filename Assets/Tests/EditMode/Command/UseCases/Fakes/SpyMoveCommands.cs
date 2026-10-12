using System.Collections.Generic;
using Features.Shared.Units;
using UnityEngine;

namespace Tests.EditMode.Command.UseCases.Fakes
{
    public sealed class SpyMoveCommands : IMoveCommands
    {
        public readonly List<string> Calls = new();
        public Vector2 LastCenter;
        public Vector2 LastSize;
        public Vector2 LastForward;
        public Vector2 LastRight;
        public Vector2 LastOrderDirection;

        public void StartPreview() => Calls.Add(nameof(StartPreview));

        public void UpdatePreview(Vector2 center, Vector2 size, Vector2 formationForward, Vector2 formationRight)
        {
            Calls.Add(nameof(UpdatePreview));
            LastCenter = center;
            LastSize = size;
            LastForward = formationForward;
            LastRight = formationRight;
        }

        public void CommitMove(Vector2 slotsOrderDirection)
        {
            Calls.Add(nameof(CommitMove));
            LastOrderDirection = slotsOrderDirection;
        }

        public void ClearPreview() => Calls.Add(nameof(ClearPreview));
    }
}
