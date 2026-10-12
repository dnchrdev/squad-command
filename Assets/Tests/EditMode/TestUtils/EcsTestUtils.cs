using Scellecs.Morpeh;
using NUnit.Framework;

namespace Tests.EditMode.TestUtils
{
    public static class EcsTestUtils
    {
        public static T Init<T>(this T system, World world) where T : IInitializer
        {
            system.World = world;
            system.OnAwake();
            return system;
        }

        public static int CountWith<T>(this World world) where T : struct, IComponent
        {
            var filter = world.Filter.With<T>().Build();
            var count = 0;
            foreach (var _ in filter)
                count++;
            return count;
        }

        public static Entity SingleWith<T>(this World world) where T : struct, IComponent
        {
            var filter = world.Filter.With<T>().Build();
            foreach (var entity in filter)
                return entity;
            throw new AssertionException($"No entity with {typeof(T).Name}");
        }
    }
}
