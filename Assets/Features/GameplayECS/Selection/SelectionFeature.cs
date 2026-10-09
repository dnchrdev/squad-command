using Features.GameplayECS.Selection.Systems;
using Scellecs.Morpeh;

namespace Features.GameplayECS.Selection
{
    public static class SelectionFeature
    {
        public static void Add(SystemsGroup group)
        {
            group.AddSystem(new ApplySelectionRequestsSystem());
            group.AddSystem(new HideSelectPreviewSystem());
            group.AddSystem(new ShowSelectPreviewSystem());
        }
    }
}