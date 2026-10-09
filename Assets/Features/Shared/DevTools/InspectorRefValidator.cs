using UnityEngine;

namespace Features.Shared.DevTools
{
    public static class InspectorRefValidator
    {
        public static bool CheckAssigned(Object value, string fieldName, Object owner)
        {
            if (value != null)
                return true;
 
            string ownerType = owner != null ? owner.GetType().Name : "UnknownOwner";
            Debug.LogError($"{ownerType}: field '{fieldName}' is not assigned in the inspector", owner);
            return false;
        }

        public static bool CheckAllAssigned(Object owner, params (Object value, string fieldName)[] fields)
        {
            bool allValid = true;
 
            foreach (var (value, fieldName) in fields)
            {
                if (value == null)
                {
                    string ownerType = owner != null ? owner.GetType().Name : "UnknownOwner";
                    Debug.LogError($"{ownerType}: field '{fieldName}' is not assigned in the inspector", owner);
                    allValid = false;
                }
            }
 
            return allValid;
        }
    }
}