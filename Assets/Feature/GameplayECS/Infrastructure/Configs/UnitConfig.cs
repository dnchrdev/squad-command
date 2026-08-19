using UnityEngine;

namespace Feature.GameplayECS.Infrastructure.Configs
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Unit Config", fileName = "UnitConfig")]
    public class UnitConfig : ScriptableObject
    {
        [field: Header("Addressable")]
        [field: SerializeField] public string AddressableKey { get; private set; }
        
        [field: Header("Move")]
        [field: SerializeField] public float MoveSpeed { get; private set; } = 3.5f;
        
        [field: Header("Health")]
        [field: SerializeField] public float MaxHealth { get; private set; } = 100f;
    }
}