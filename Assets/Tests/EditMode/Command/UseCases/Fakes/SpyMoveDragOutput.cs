using System.Collections.Generic;
using Features.Command.UseCases.Ports;
using UnityEngine;

namespace Tests.EditMode.Command.UseCases.Fakes
{
    public sealed class SpyMoveDragOutput : IMoveDragOutput
    {
        public readonly List<string> Calls = new();
        public Vector2 LastStart;
        public Vector2 LastEnd;

        public void ShowDrag(Vector2 screenStart)
        {
            Calls.Add(nameof(ShowDrag));
            LastStart = screenStart;
        }

        public void UpdateDrag(Vector2 screenStart, Vector2 screenEnd)
        {
            Calls.Add(nameof(UpdateDrag));
            LastStart = screenStart;
            LastEnd = screenEnd;
        }

        public void HideDrag() => Calls.Add(nameof(HideDrag));
    }
}
