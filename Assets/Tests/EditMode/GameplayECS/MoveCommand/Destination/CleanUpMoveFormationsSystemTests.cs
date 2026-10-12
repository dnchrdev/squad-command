using System.Collections.Generic;
using Features.GameplayECS.MoveCommand.Destination.Components;
using Features.GameplayECS.MoveCommand.Destination.Systems;
using NUnit.Framework;
using Scellecs.Morpeh;
using Tests.EditMode.TestUtils;

namespace Tests.EditMode.GameplayECS.MoveCommand.Destination
{
    public class CleanUpMoveFormationsSystemTests
    {
        private const float DT = 0.016f;

        private World _world;
        private CleanUpMoveFormationsSystem _system;

        [SetUp]
        public void SetUp()
        {
            _world = World.Create();
            _system = new CleanUpMoveFormationsSystem().Init(_world);
        }

        [TearDown]
        public void TearDown() => _world.Dispose();

        [Test]
        public void OnUpdate_NoUnitsLeft_RemovesFormation()
        {
            CreateAssignedFormation(0, 1);

            Run();

            Assert.That(_world.CountWith<MoveDestinationFormation>(), Is.Zero);
        }

        [Test]
        public void OnUpdate_NoSlotsLeft_RemovesFormation()
        {
            CreateAssignedFormation(1, 0);

            Run();

            Assert.That(_world.CountWith<MoveDestinationFormation>(), Is.Zero);
        }

        [Test]
        public void OnUpdate_UnitsAndSlotsLeft_KeepsFormation()
        {
            CreateAssignedFormation(1, 1);

            Run();

            Assert.That(_world.CountWith<MoveDestinationFormation>(), Is.EqualTo(1));
        }

        [Test]
        public void OnUpdate_FormationStillApplying_IsKept()
        {
            var formation = CreateAssignedFormation(0, 0);
            _world.GetStash<ApplyDestinationFormationTag>().Set(formation);

            Run();

            Assert.That(_world.CountWith<MoveDestinationFormation>(), Is.EqualTo(1));
        }

        private Entity CreateAssignedFormation(int unitsCount, int slotsCount)
        {
            var formation = _world.CreateEntity();
            _world.GetStash<MoveDestinationFormation>().Set(formation, new MoveDestinationFormation
            {
                Units = CreateEntities(unitsCount),
                Slots = CreateEntities(slotsCount)
            });
            _world.GetStash<DestinationSlotsAssignedTag>().Set(formation);
            return formation;
        }

        private List<Entity> CreateEntities(int count)
        {
            var entities = new List<Entity>();
            for (int i = 0; i < count; i++)
                entities.Add(_world.CreateEntity());
            return entities;
        }

        private void Run()
        {
            _world.Commit();
            _system.OnUpdate(DT);
            _world.Commit();
        }
    }
}
