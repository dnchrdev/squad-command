using System.Collections.Generic;
using Features.Shared.Units;

namespace Tests.EditMode.Command.UseCases.Fakes
{
    public sealed class StubUnitQuery : IUnitQuery
    {
        public readonly List<UnitSnapshot> All = new();
        public readonly List<UnitSnapshot> Selected = new();

        public void GetAllUnits(List<UnitSnapshot> result)
        {
            result.Clear();
            result.AddRange(All);
        }

        public void GetSelectedUnits(List<UnitSnapshot> result)
        {
            result.Clear();
            result.AddRange(Selected);
        }
    }
}
