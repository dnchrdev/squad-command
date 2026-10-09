using Features.GameplayECS.Navigation.Systems.Ports;
using Features.GameplayECS.Navigation.Systems;
using Scellecs.Morpeh;

namespace Features.GameplayECS.Navigation
{
    public static class NavigationFeature
    {
        public static void Add(SystemsGroup group, IPathService pathService)
        {
            group.AddSystem(new ApplyDestinationRequestSystem(pathService));
            group.AddSystem(new CalculateNavigationDirectionSystem());
        }
    }
}