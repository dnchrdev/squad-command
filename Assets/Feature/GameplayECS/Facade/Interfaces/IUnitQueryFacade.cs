using Feature.GameplayECS.Common;
using Scellecs.Morpeh;

namespace Feature.GameplayECS.Facade.Interfaces
{
    public interface IUnitQueryFacade
    {
        public Filter AllUnits { get; }
        public Filter PreviewSelectedUnits { get; }
        public Filter SelectedUnits { get; }
        public ref Position GetPosition(Entity entity);
    }
}