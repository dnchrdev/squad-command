using System;
using Feature.GameplayECS.View;
using Scellecs.Morpeh;
using UnityEngine;

namespace Feature.GameplayECS.Movement.Links
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