using System;
using Features.Command.Entities;
using Features.UI.Infrastructure;

namespace Features.Command.Infrastructure
{
    [Serializable]
    public class CommandButtonEntry
    {
        public CommandType Type;
        public ElevationButton Button;
    }
}