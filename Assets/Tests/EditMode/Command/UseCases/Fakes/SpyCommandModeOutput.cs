using System.Collections.Generic;
using Features.Command.Entities;
using Features.Command.UseCases.Ports;

namespace Tests.EditMode.Command.UseCases.Fakes
{
    public sealed class SpyCommandModeOutput : ICommandModeOutput
    {
        public readonly List<CommandType> Shown = new();

        public void ShowActiveCommand(CommandType type) => Shown.Add(type);
    }
}
