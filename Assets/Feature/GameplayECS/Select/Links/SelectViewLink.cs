using System;
using Feature.GameplayECS.View;
using Scellecs.Morpeh;
using UnityEngine;

namespace Feature.GameplayECS.Select.Links
{
    public class SelectViewLink : MonoBehaviour, IEntityLink
    {
        [SerializeField] private SelectView _view;

        public void Link(Entity entity, World world)
        {
            world.GetStash<SelectViewComponent>().Set(entity, new SelectViewComponent { Value = _view });
        }

        public void Unlink(Entity entity, World world)
        {
            world.GetStash<SelectViewComponent>().Remove(entity); 
        }
    }
}