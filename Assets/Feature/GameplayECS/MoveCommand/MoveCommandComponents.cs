using System.Collections.Generic;
using Scellecs.Morpeh;
using UnityEngine;

namespace Feature.GameplayECS.MoveCommand
{
    public struct MovePreviewTag : IComponent
    {
    }

    public struct MovePreviewViewComponent : IComponent
    {
        public MovePreviewView Value;
    }

    public struct MovePreviewShowed : IComponent
    {
    }

    public struct MovePreviewSlotPosition : IComponent
    {
        public Vector3 Value;
    }
    
    public struct MovePreviewState : IComponent
    {
        public List<Entity> ActiveSlots;

        /// <summary>
        /// Последняя посчитанная целевая сетка позиций (мировые координаты),
        /// актуальная относительно последнего полученного MovePreviewRequest
        /// (т.е. учитывает самый свежий formationForward/formationRight).
        /// Источник истины для позиционирования слотов — и уже существующих,
        /// и создающихся асинхронно.
        /// </summary>
        public List<Vector3> LastGrid;

        /// <summary>
        /// Флаг "сетка изменилась в этом кадре и слоты нужно переставить".
        /// Выставляется MovePreviewShowSystem, потребляется
        /// MovePreviewSlotRepositionSystem и сбрасывается им же.
        /// </summary>
        public bool GridDirty;
    }
}