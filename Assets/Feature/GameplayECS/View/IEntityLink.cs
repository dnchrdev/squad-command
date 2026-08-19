using Scellecs.Morpeh;

namespace Feature.GameplayECS.View
{
    public interface IEntityLink
    {
        void Link(Entity entity, World world);
        void Unlink(Entity entity, World world);
    }
}