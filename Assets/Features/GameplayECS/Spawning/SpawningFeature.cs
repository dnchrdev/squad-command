using Features.GameplayECS.Spawning.Systems;
using Scellecs.Morpeh;

namespace Features.GameplayECS.Spawning
{
    public static class SpawningFeature
    {
        public static void Add(SystemsGroup group)
        {
            group.AddInitializer(new SpawnOnInitInitializer());
        }
    }
}