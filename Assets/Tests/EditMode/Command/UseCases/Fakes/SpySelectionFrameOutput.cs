using System.Collections.Generic;
using Features.Command.UseCases.Ports;
using UnityEngine;

namespace Tests.EditMode.Command.UseCases.Fakes
{
    public sealed class SpySelectionFrameOutput : ISelectionFrameOutput
    {
        public readonly List<string> Calls = new();
        public Rect LastRect;

        public void ShowFrame(Rect screenRect)
        {
            Calls.Add(nameof(ShowFrame));
            LastRect = screenRect;
        }

        public void UpdateFrame(Rect screenRect)
        {
            Calls.Add(nameof(UpdateFrame));
            LastRect = screenRect;
        }

        public void HideFrame() => Calls.Add(nameof(HideFrame));
    }
}
