using Features.Command.Entities;

namespace Features.Command.UseCases.Ports
{
    public interface ICommandModeOutput
    {
        void ShowActiveCommand(CommandType type);
    }
}