using System;
using Features.GameplayECS.Common.View;
using Features.GameplayECS.Synchronization.Components;
using Features.GameplayECS.Synchronization.View;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.GameplayECS.Synchronization.Links
{
    [RequireComponent(typeof(UnitMotor))]
    public class UnitMotorLink: MonoBehaviour, IEntityLink
    {
        [SerializeField] private UnitMotor _motor;
        
        private void OnValidate()
        {
            _motor = GetComponent<UnitMotor>();
            if(_motor == null) throw new NullReferenceException("UnitMotor is null");
        }

        public void Link(Entity entity, World world)
        {
            world.GetStash<UnitMotorComponent>().Set(entity, new UnitMotorComponent{ Value = _motor});
        }

        public void Unlink(Entity entity, World world)
        {
            world.GetStash<UnitMotorComponent>().Remove(entity);
        }
    }
}