using UnityEngine;

namespace Feature.Command.Infrastructure.Configs
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
    }
}
