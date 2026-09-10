using System;
using UnityEngine;

namespace Feature.GameplayECS.Movement
{
    [RequireComponent(typeof(Rigidbody))]
    public class UnitMotor: MonoBehaviour, IUnitMotor
    {
        [SerializeField] private Transform _motorTransform;  
        [SerializeField] private Rigidbody _rb;
        public Vector3 Position => _motorTransform.position;
        public Quaternion Rotation =>  _motorTransform.rotation;
        public Vector3 Velocity => _rb.linearVelocity;

        private void OnValidate()
        {
            _rb = GetComponent<Rigidbody>();
            
            if(_rb == null) throw new NullReferenceException("Rigidbody is null");
            if(_motorTransform == null) throw new NullReferenceException("MotorTransform is null");
            
            _rb.useGravity = false;
        }
        
        public void SetPosition(Vector3 position)
        {
            gameObject.transform.position = position + _motorTransform.localPosition;
        }

        public void SetVelocity(Vector3 velocity)
        {
            _rb.linearVelocity = velocity;
        }
        
    }
}