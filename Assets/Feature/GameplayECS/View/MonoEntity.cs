using Scellecs.Morpeh;
using UnityEngine;

namespace Feature.GameplayECS.View
{
    public class MonoEntity: MonoBehaviour
    {
        private IEntityLink[] _links;
        public Entity Entity  { get; private set; }
        public World World  { get; private set; }
        
        public void Bind(Entity entity,  World world)
        {
            Entity = entity;
            World = world;

            _links = GetComponentsInChildren<IEntityLink>();

            foreach (var link in _links)
            {
                link.Link(entity, world);
            }
        }

        public void Unbind()
        {
            foreach (var link in _links)
                link.Unlink(Entity,  World);
            
            Entity = default;
            World = null;
        }
    }
}