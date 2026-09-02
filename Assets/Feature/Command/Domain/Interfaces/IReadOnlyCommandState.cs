using Feature.Command.Domain.Data;

namespace Feature.Command.Domain.Interfaces
{
    public interface IReadOnlyCommandState
    {
        CommandType CommandType { get; }
    }
}