using System.Collections.Generic;
using Features.GameplayECS.MoveCommand.Destination.Systems;
using NUnit.Framework;
using UnityEngine;

namespace Tests.EditMode.GameplayECS.MoveCommand.Destination
{
    public class SlotAssignmentSolverTests
    {
        private const int UNASSIGNED = -1;

        private SlotAssignmentSolver _solver;
        private List<int> _result;

        [SetUp]
        public void SetUp()
        {
            _solver = new SlotAssignmentSolver();
            _result = new List<int>();
        }

        [Test]
        public void Solve_EqualCounts_MinimizesTotalDistance()
        {
            var units = new[] { X(0f), X(1f), X(2f) };
            var slots = new[] { X(2f), X(0f), X(1f) };

            _solver.Solve(units, slots, _result);

            Assert.That(_result, Is.EqualTo(new[] { 1, 2, 0 }));
        }

        [Test]
        public void Solve_MoreUnitsThanSlots_GivesSlotToNearestUnit()
        {
            var units = new[] { X(0f), X(10f), X(5f) };
            var slots = new[] { X(9f) };

            _solver.Solve(units, slots, _result);

            Assert.That(_result, Is.EqualTo(new[] { UNASSIGNED, 0, UNASSIGNED }));
        }

        [Test]
        public void Solve_MoreSlotsThanUnits_PicksNearestSlot()
        {
            var units = new[] { X(0f) };
            var slots = new[] { X(5f), X(1f) };

            _solver.Solve(units, slots, _result);

            Assert.That(_result, Is.EqualTo(new[] { 1 }));
        }

        [Test]
        public void Solve_IgnoresHeight()
        {
            var units = new[] { new Vector3(0f, 100f, 0f) };
            var slots = new[] { new Vector3(3f, 0f, 0f), new Vector3(0f, 0f, 1f) };

            _solver.Solve(units, slots, _result);

            Assert.That(_result, Is.EqualTo(new[] { 1 }));
        }

        [Test]
        public void Solve_NoSlots_LeavesAllUnitsUnassigned()
        {
            var units = new[] { X(0f), X(1f) };

            _solver.Solve(units, new Vector3[0], _result);

            Assert.That(_result, Is.EqualTo(new[] { UNASSIGNED, UNASSIGNED }));
        }

        [Test]
        public void Solve_CalledAgain_ReplacesPreviousResult()
        {
            _solver.Solve(new[] { X(0f), X(1f), X(2f) }, new[] { X(0f), X(1f), X(2f) }, _result);

            _solver.Solve(new[] { X(0f) }, new[] { X(4f), X(0f) }, _result);

            Assert.That(_result, Is.EqualTo(new[] { 1 }));
        }

        private static Vector3 X(float x) => new Vector3(x, 0f, 0f);
    }
}
