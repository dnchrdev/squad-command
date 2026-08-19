using Scellecs.Morpeh;

namespace Feature.GameplayECS.Select
{
    public struct SelectSelfRequest: IComponent { }
    public struct UnselectSelfRequest: IComponent { }
    public struct Selected : IComponent { }
    public struct SelectViewComponent : IComponent { public SelectView Value;}
    public struct SelectedViewShowed : IComponent { }
    

}