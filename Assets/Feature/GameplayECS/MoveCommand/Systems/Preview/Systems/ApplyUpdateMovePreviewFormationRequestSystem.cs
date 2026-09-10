using System;
using Feature.GameplayECS.Common;
using Scellecs.Morpeh;
using UnityEngine;

namespace Feature.GameplayECS.MoveCommand.Systems.Preview.Systems
{
    public class ApplyUpdateMovePreviewFormationRequestSystem : ISystem
    {
        public World World { get; set; }

        private const float SLOT_SPACING = 1.2f;
        private const float SQUEEZE_MIN_BLEND_DIAGONAL = 4f;
        private const float SQUEEZE_MAX_BLEND_DIAGONAL = 14f;

        private Filter _updateMovePreviewFormationRequestFilter;
        private Filter _movePreviewFormationFilter;
        private Filter _pendingSlotsFilter;

        private Filter _selectedWithoutSlotFilter;

        private Stash<UpdateMovePreviewFormationRequest> _updateMoveFormationRequestStash;
        private Stash<MovePreviewFormation> _movePreviewFormationStash;
        private Stash<Position> _positionStash;


        public void OnAwake()
        {
            _updateMovePreviewFormationRequestFilter = World.Filter
                .With<UpdateMovePreviewFormationRequest>()
                .Build();

            _movePreviewFormationFilter = World.Filter
                .With<MovePreviewFormation>()
                //.With<FormationSlots>()
                .Build();

            _updateMoveFormationRequestStash = World.GetStash<UpdateMovePreviewFormationRequest>();
            _movePreviewFormationStash = World.GetStash<MovePreviewFormation>();
            _positionStash = World.GetStash<Position>();
        }

        public void OnUpdate(float deltaTime)
        {
            bool hasUpdateFormationRequest = false;
            Entity request = default;

            foreach (var updateRequest in _updateMovePreviewFormationRequestFilter)
            {
                hasUpdateFormationRequest = true;
                request = updateRequest;
                break;
            }

            if (hasUpdateFormationRequest == false) return;

            foreach (var previewFormation in _movePreviewFormationFilter)
            {
                ref var requestData = ref _updateMoveFormationRequestStash.Get(request);
                ref var movePreviewFormation = ref _movePreviewFormationStash.Get(previewFormation);

                int slotsCount = movePreviewFormation.Slots.Count;
                Vector2 center = requestData.Center;
                Vector2 size = requestData.Size;
                Vector3 forward = requestData.FormationForward;
                Vector3 right = requestData.FormationRight;

                Vector3[] grid = GenerateAdaptiveFormationGrid(slotsCount, center, size, forward, right);

                for (int i = 0; i < grid.Length; i++)
                {
                    Entity slot = movePreviewFormation.Slots[i];
                    _positionStash.Set(slot, new Position { Value = grid[i] + new Vector3(0f, 0.01f, 0f) });
                }

                break;
            }

            RequestUtils.ConsumeAllRequests(World, _updateMovePreviewFormationRequestFilter,
                _updateMoveFormationRequestStash);
        }

        private static Vector3[] GenerateAdaptiveFormationGrid(int unitsCount, Vector2 center, Vector2 size,
            Vector2 formationForward, Vector2 formationRight)
        {
            if (unitsCount <= 0) return Array.Empty<Vector3>();

            Vector2 right = formationRight.sqrMagnitude > 0.0001f ? formationRight.normalized : Vector2.right;
            Vector2 forward = formationForward.sqrMagnitude > 0.0001f ? formationForward.normalized : Vector2.up;

            ResolveFormationGridDimensions(unitsCount, size, out int columns, out int rows);

            var result = new Vector3[unitsCount];
            int placed = 0;

            for (int row = rows - 1; row >= 0 && placed < unitsCount; row--)
            {
                float forwardOffset = (row - (rows - 1) * 0.5f) * SLOT_SPACING;
                for (int col = 0; col < columns && placed < unitsCount; col++)
                {
                    float rightOffset = (col - (columns - 1) * 0.5f) * SLOT_SPACING;
                    Vector2 flatPosition = center + right * rightOffset + forward * forwardOffset;
                    result[placed] = new Vector3(flatPosition.x, 0f, flatPosition.y);
                    placed++;
                }
            }

            return result;
        }

        private static void ResolveFormationGridDimensions(int unitsCount, Vector2 size, out int columns, out int rows)
        {
            float rawAspect = Mathf.Max(size.x, 0.01f) / Mathf.Max(size.y, 0.01f);
            float diagonal = Mathf.Sqrt(size.x * size.x + size.y * size.y);
            float blend = Mathf.InverseLerp(SQUEEZE_MIN_BLEND_DIAGONAL, SQUEEZE_MAX_BLEND_DIAGONAL, diagonal);
            float blendedAspect = Mathf.Exp(Mathf.Lerp(0f, Mathf.Log(rawAspect), blend));
            float bestScore = float.PositiveInfinity;
            int bestColumns = Mathf.CeilToInt(Mathf.Sqrt(unitsCount));
            int bestRows = Mathf.CeilToInt(unitsCount / (float)bestColumns);

            for (int c = 1; c <= unitsCount; c++)
            {
                int r = Mathf.CeilToInt(unitsCount / (float)c);
                if (c * r < unitsCount) continue;
                float score = Mathf.Abs(Mathf.Log(c / (float)r) - Mathf.Log(blendedAspect));
                if (score < bestScore)
                {
                    bestScore = score;
                    bestColumns = c;
                    bestRows = r;
                }
            }

            columns = bestColumns;
            rows = bestRows;
        }

        public void Dispose()
        {
        }
    }
}