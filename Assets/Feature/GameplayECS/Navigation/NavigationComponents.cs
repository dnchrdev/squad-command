using System.Collections.Generic;
using Scellecs.Morpeh;
using UnityEngine;

namespace Feature.GameplayECS.Navigation
{
    public struct Destination : IComponent
    {
        public Vector3 Target;
        public Vector3[] Corners;
        public int CurrentCornerIndex;
        public bool HasPath;
    }

    public struct NavigationDirection : IComponent
    {
        public Vector3 Value;
    }

    public struct SteeringDirection : IComponent
    {
        public Vector3 Value;
    }

    public struct UnitRadius : IComponent
    {
        public float Value;
    }
    
    public struct SquadSlotClaim : IComponent
    {
        public Entity OrderEntity; // на какой SquadFormationOrder ссылается этот юнит
    }
    
    /// <summary>
    /// Ставится на ОТДЕЛЬНУЮ entity-приказ (не на юнитов) при выдаче группового
    /// приказа на движение. В отличие от одноразового MoveOrderRequest, живёт
    /// НЕСКОЛЬКО КАДРОВ — пока все юниты группы не окупируют свои слоты.
    ///
    /// Как это используется SquadFormationSystem:
    /// - PendingUnits — юниты, которые ЕЩЁ НЕ заняли слот. Каждый кадр всем им
    ///   рассылается ОДИН И ТОТ ЖЕ текущий целевой слот (CurrentTargetSlot) —
    ///   именно поэтому все "сбегаются" к одной точке одновременно, а не у
    ///   каждого сразу свой уникальный слот.
    /// - Когда кто-то из PendingUnits фактически ДОХОДИТ до CurrentTargetSlot —
    ///   слот "запирается" за ним: он переносится из PendingUnits в занятые
    ///   (просто удаляется из PendingUnits), CurrentTargetSlot убирается из
    ///   RemainingSlots, и на следующем кадре пересчитывается новый
    ///   CurrentTargetSlot (самый дальний от центроида ОСТАВШИХСЯ PendingUnits).
    /// - Если PendingUnits опустел — приказ завершён, entity удаляется.
    /// </summary>
    
    public struct SquadFormationOrder : IComponent
    {
        public Vector3 Center;
 
        // юниты, которые ещё не заняли свой слот — им всем каждый кадр
        // рассылается один и тот же CurrentTargetSlot
        public List<Entity> PendingUnits;
 
        // ещё не занятые слоты формейшна (кольца вокруг Center)
        public List<Vector3> RemainingSlots;
 
        // текущая общая цель для всех PendingUnits; пересчитывается, когда кто-то её занимает
        public Vector3 CurrentTargetSlot;
        public bool HasTarget;
 
        // направление подхода группы к Center, вычисленное ОДИН РАЗ в момент создания
        // приказа (через NavMesh corners[last]-corners[last-1]) и переиспользуемое на
        // всех последующих пересчётах цели — чтобы порядок "дальше по ходу" не менялся
        // от итерации к итерации из-за дрейфа центроида после выбывания юнитов
        public Vector3 ApproachDirection;
    }
}