using Feature.GameplayECS.Common;
using Scellecs.Morpeh;

namespace Feature.GameplayECS.CommandProcessing
{
    public interface IUnitQueryFacade
    {
        public Filter AllUnits { get; }
        public Filter PreviewSelectedUnits { get; }
        public ref Position GetPosition(Entity entity);
    }
}