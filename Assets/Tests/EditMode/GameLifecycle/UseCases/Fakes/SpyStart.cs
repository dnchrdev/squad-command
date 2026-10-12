using System.Collections.Generic;
using Features.Shared.Lifecycle;

namespace Tests.EditMode.GameLifecycle.UseCases.Fakes
{
    public sealed class SpyStart : ICameraStart, ICommandStart
    {
        private readonly string _name;
        private readonly List<string> _log;

        public SpyStart(string name, List<string> log)
        {
            _name = name;
            _log = log;
        }

        public void Start() => _log.Add(_name);
    }
}
