using System.Collections.Generic;
using Features.GameplayECS.MoveCommand.Components;
using Features.GameplayECS.MoveCommand.Preview.Components;
using Features.GameplayECS.MoveCommand.Preview.Requests;
using Features.GameplayECS.MoveCommand.Preview.Systems;
using NUnit.Framework;
using Scellecs.Morpeh;
using Tests.EditMode.TestUtils;
using UnityEngine;

namespace Tests.EditMode.GameplayECS.MoveCommand.Preview
{
    public class ApplyUpdateMovePreviewFormationRequestSystemTests
    {
        private const float DT = 0.016f;

        private World _world;
        private ApplyUpdateMovePreviewFormationRequestSystem _system;

        [SetUp]
        public void SetUp()
        {
            _world = World.Create();
            _system = new ApplyUpdateMovePreviewFormationRequestSystem().Init(_world);
        }

        [TearDown]
        public void TearDown() => _world.Dispose();

        [Test]
        public void OnUpdate_FourSlotsAndShortDrag_PlacesSquareGridAroundCenter()
        {
            var slots = CreateFormation(4);
            SendRequest(new Vector2(10f, 20f), Vector2.zero);

            Run();

            VectorAssert.AreEqual(new Vector3(9.4f, 0.01f, 20.6f), PositionOf(slots[0]));
            VectorAssert.AreEqual(new Vector3(10.6f, 0.01f, 20.6f), PositionOf(slots[1]));
            VectorAssert.AreEqual(new Vector3(9.4f, 0.01f, 19.4f), PositionOf(slots[2]));
            VectorAssert.AreEqual(new Vector3(10.6f, 0.01f, 19.4f), PositionOf(slots[3]));
        }

        [Test]
        public void OnUpdate_ThreeSlotsAndWideDrag_PlacesSingleLine()
        {
            var slots = CreateFormation(3);
            SendRequest(Vector2.zero, new Vector2(14f, 0f));

            Run();

            VectorAssert.AreEqual(new Vector3(-1.2f, 0.01f, 0f), PositionOf(slots[0]));
            VectorAssert.AreEqual(new Vector3(0f, 0.01f, 0f), PositionOf(slots[1]));
            VectorAssert.AreEqual(new Vector3(1.2f, 0.01f, 0f), PositionOf(slots[2]));
        }

        [Test]
        public void OnUpdate_ConsumesRequest()
        {
            CreateFormation(1);
            SendRequest(Vector2.zero, Vector2.zero);

            Run();

            Assert.That(_world.CountWith<UpdateMovePreviewFormationRequest>(), Is.Zero);
        }

        private List<Entity> CreateFormation(int slotsCount)
        {
            var slots = new List<Entity>();
            for (int i = 0; i < slotsCount; i++)
            {
                var slot = _world.CreateEntity();
                _world.GetStash<MovePreviewSlotTag>().Set(slot);
                slots.Add(slot);
            }

            _world.GetStash<MovePreviewFormation>().Set(_world.CreateEntity(),
                new MovePreviewFormation { Slots = new List<Entity>(slots) });
            return slots;
        }

        private void SendRequest(Vector2 center, Vector2 size)
        {
            _world.GetStash<UpdateMovePreviewFormationRequest>().Set(_world.CreateEntity(),
                new UpdateMovePreviewFormationRequest
                {
                    Center = center,
                    Size = size,
                    FormationForward = Vector2.up,
                    FormationRight = Vector2.right
                });
        }

        private Vector3 PositionOf(Entity slot) => _world.GetStash<MoveSlotPosition>().Get(slot).Value;

        private void Run()
        {
            _world.Commit();
            _system.OnUpdate(DT);
            _world.Commit();
        }
    }
}
