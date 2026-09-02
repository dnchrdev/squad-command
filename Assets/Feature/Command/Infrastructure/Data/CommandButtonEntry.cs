using System;
using Feature.Command.Domain.Data;
using Feature.UI.Adapter;

namespace Feature.Command.Infrastructure.Data
{
    [Serializable]
    public class CommandButtonEntry
    {
        public CommandType Type;
        public ElevationButton Button;
    }
}