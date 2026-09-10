using Scellecs.Morpeh;

namespace Feature.GameplayECS.Common
{
    public class RequestUtils
    {
        public static void ConsumeAllRequests<T>(World world, Filter filter, Stash<T> stash) where T : struct, IComponent
        {
            foreach (var entity in filter)
            {
                stash.Remove(entity);
                world.RemoveEntity(entity);
            }
        }
    }
}