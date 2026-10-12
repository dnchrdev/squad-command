using Features.Command.Entities;
using NUnit.Framework;
using Tests.EditMode.TestUtils;
using UnityEngine;

namespace Tests.EditMode.Command.Entities
{
    public class FormationTests
    {
        private static readonly Vector2 UnitsCenter = new Vector2(0f, 0f);
        private static readonly Vector2 DragStart = new Vector2(0f, 10f);

        [Test]
        public void Constructor_FacesFromUnitsToDragStart()
        {
            var formation = new Formation(UnitsCenter, DragStart);

            VectorAssert.AreEqual(DragStart, formation.Center);
            VectorAssert.AreEqual(new Vector2(0f, 1f), formation.Forward);
            VectorAssert.AreEqual(new Vector2(1f, 0f), formation.Right);
        }

        [Test]
        public void UpdateDrag_LongDrag_RotatesAndResizes()
        {
            var formation = new Formation(UnitsCenter, DragStart);

            formation.UpdateDrag(DragStart, new Vector2(4f, 10f));

            VectorAssert.AreEqual(new Vector2(-1f, 0f), formation.Forward);
            VectorAssert.AreEqual(new Vector2(0f, 1f), formation.Right);
            VectorAssert.AreEqual(new Vector2(0f, 4f), formation.Size);
        }

        [Test]
        public void UpdateDrag_ShortDrag_KeepsDirection()
        {
            var formation = new Formation(UnitsCenter, DragStart);

            formation.UpdateDrag(DragStart, new Vector2(0.3f, 10f));

            VectorAssert.AreEqual(new Vector2(0f, 1f), formation.Forward);
            VectorAssert.AreEqual(new Vector2(0.3f, 0f), formation.Size);
        }

        [Test]
        public void GetDirectionToCenter_FromUnitsCenter_PointsToFormationCenter()
        {
            var formation = new Formation(UnitsCenter, DragStart);

            var direction = formation.GetDirectionToCenter(UnitsCenter);

            VectorAssert.AreEqual(new Vector2(0f, 1f), direction);
        }

        [Test]
        public void Constructor_DragStartAtUnitsCenter_KeepsDefaultDirection()
        {
            var formation = new Formation(DragStart, DragStart);

            VectorAssert.AreEqual(Vector2.up, formation.Forward);
            VectorAssert.AreEqual(Vector2.right, formation.Right);
        }

        [Test]
        public void UpdateDrag_DragExactlyAtRotationThreshold_KeepsDirection()
        {
            var formation = new Formation(UnitsCenter, DragStart);

            formation.UpdateDrag(DragStart, new Vector2(0.5f, 10f));

            VectorAssert.AreEqual(new Vector2(0f, 1f), formation.Forward);
            VectorAssert.AreEqual(new Vector2(0.5f, 0f), formation.Size);
        }

        [Test]
        public void UpdateDrag_MovesCenterToDragStart()
        {
            var formation = new Formation(UnitsCenter, DragStart);
            var newStart = new Vector2(4f, 4f);

            formation.UpdateDrag(newStart, newStart);

            VectorAssert.AreEqual(newStart, formation.Center);
        }
    }
}
