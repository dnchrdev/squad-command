using Features.Command.Entities;
using Features.Command.UseCases.Ports;

namespace Tests.EditMode.Command.UseCases.Fakes
{
    public sealed class DummyCommandModeOutput : ICommandModeOutput
    {
        public void ShowActiveCommand(CommandType type) { }
    }
}