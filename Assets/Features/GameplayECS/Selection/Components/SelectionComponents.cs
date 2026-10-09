using Features.GameplayECS.Selection.View;
using Scellecs.Morpeh;

namespace Features.GameplayECS.Selection.Components
{
    public struct SelectViewComponent : IComponent { public SelectView Value; }
    public struct SelectedViewShowedTag : IComponent { }
    public struct SelectPreviewTag : IComponent { }
    public struct UnselectPreviewTag : IComponent { }
}