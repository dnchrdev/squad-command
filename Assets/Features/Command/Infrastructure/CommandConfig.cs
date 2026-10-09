using Features.Command.Entities;
using UnityEngine;

namespace Features.Command.Infrastructure
{
    [CreateAssetMenu(fileName = "CommandConfig", menuName = "Scriptable Objects/CommandConfig")]
    public class CommandConfig : ScriptableObject
    {
        [field: Header("Select")]
        [field: SerializeField, Range(0f, 25f)] public float SingleSelectMaxThreshold { get; set; } = 25f;
        [field: SerializeField, Range(25f, 150f)] public float SingleSelectCastDistance { get; private set; } = 150f;
        [field: SerializeField, Range(0.01f, 1f)] public float SingleSelectCastRadius { get; private set; } = 0.25f;
        
        [field: Header("LayerMask")]
        [field: SerializeField] public LayerMask UnitMask { get; private set; }
        [field: SerializeField] public LayerMask GroundMask { get; private set; }
        
        [field: Header("Ground")]
        [field: SerializeField, Range(10f, 1000f)] public float GroundRaycastDistance { get; private set; } = 500f;

        public CommandSettings ToSettings() => new CommandSettings(SingleSelectMaxThreshold);
    }
}
