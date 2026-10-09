using System.Collections.Generic;

namespace Features.Shared.Units
{
    public interface IUnitQuery
    {
        void GetAllUnits(List<UnitSnapshot> result);
        void GetSelectedUnits(List<UnitSnapshot> result);
    }
}