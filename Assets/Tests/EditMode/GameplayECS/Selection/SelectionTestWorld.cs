using System.Collections.Generic;
using Features.GameplayECS.Common.Components;
using Features.GameplayECS.Selection.Components;
using Scellecs.Morpeh;
using Tests.EditMode.TestUtils;
using UnityEngine;

namespace Tests.EditMode.GameplayECS.Selection
{
    public sealed class SelectionTestWorld
    {
        private readonly List<GameObject> _createdObjects = new();
        private readonly Dictionary<Entity, GameObject> _visuals = new();

        public World World { get; }

        public SelectionTestWorld()
        {
            World = World.Create();
        }

        public Entity CreateUnit()
        {
            var unit = World.CreateEntity();
            World.GetStash<UnitTag>().Set(unit);
            return unit;
        }

        public Entity CreateUnitWithView()
        {
            var unit = CreateUnit();
            var view = SelectViewTestFactory.Create(out var visual);
            _createdObjects.Add(view.gameObject);
            _visuals[unit] = visual;
            World.GetStash<SelectViewComponent>().Set(unit, new SelectViewComponent { Value = view });
            return unit;
        }

        public void ShowVisual(Entity unit)
        {
            _visuals[unit].SetActive(true);
            World.GetStash<SelectedViewShowedTag>().Set(unit);
        }

        public bool IsVisualShown(Entity unit) => _visuals[unit].activeSelf;

        public void Dispose()
        {
            foreach (var createdObject in _createdObjects)
                Object.DestroyImmediate(createdObject);
            World.Dispose();
        }
    }
}
