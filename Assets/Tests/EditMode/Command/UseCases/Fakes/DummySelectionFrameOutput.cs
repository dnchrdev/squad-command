using Features.Command.Entities;
using Features.Command.UseCases.Ports;
using UnityEngine;

namespace Tests.EditMode.Command.UseCases.Fakes
{
    public sealed class DummySelectionFrameOutput : ISelectionFrameOutput
    {
        public void ShowFrame(Rect screenRect) { }
        public void UpdateFrame(Rect screenRect) { }
        public void HideFrame() { }
    }
}