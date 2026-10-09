using Scellecs.Morpeh;

namespace Features.GameplayECS.Common.View
{
    public interface IEntityLink
    {
        void Link(Entity entity, World world);
        void Unlink(Entity entity, World world);
    }
}