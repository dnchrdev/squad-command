using Features.GameplayECS.Selection.View;
using UnityEditor;
using UnityEngine;

namespace Tests.EditMode.TestUtils
{
    public static class SelectViewTestFactory
    {
        private const string SELECTED_VISUAL_FIELD = "_selectedVisual";

        public static SelectView Create(out GameObject visual)
        {
            var root = new GameObject(nameof(SelectView));
            visual = new GameObject(SELECTED_VISUAL_FIELD);
            visual.transform.SetParent(root.transform);
            visual.SetActive(false);

            var view = root.AddComponent<SelectView>();
            var serialized = new SerializedObject(view);
            serialized.FindProperty(SELECTED_VISUAL_FIELD).objectReferenceValue = visual;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            return view;
        }
    }
}
