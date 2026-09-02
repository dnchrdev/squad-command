using UnityEngine;

namespace Feature.Shared.DevTools
{
    /// <summary>
    /// Утилита для проверки, что ссылки, назначаемые через инспектор (SerializeField),
    /// действительно заполнены. Предназначена для вызова из Awake()/OnValidate().
    /// </summary>
    public static class InspectorRefValidator
    {
        /// <summary>
        /// Проверяет одну ссылку. Возвращает true, если ссылка валидна (не null).
        /// В случае null пишет в консоль ошибку с указанием владельца, поля и класса.
        /// </summary>
        /// <param name="value">Проверяемое значение (Unity-объект).</param>
        /// <param name="fieldName">Имя поля — передавать через nameof(_fieldName).</param>
        /// <param name="owner">Объект-владелец поля (для клика в консоли, ведущего на объект в сцене).</param>
        public static bool CheckAssigned(Object value, string fieldName, Object owner)
        {
            if (value != null)
                return true;
 
            string ownerType = owner != null ? owner.GetType().Name : "UnknownOwner";
            Debug.LogError($"{ownerType}: field '{fieldName}' is not assigned in the inspector", owner);
            return false;
        }
 
        /// <summary>
        /// Проверяет несколько ссылок сразу, каждую по отдельности логируя при отсутствии.
        /// Возвращает true, только если все ссылки валидны.
        /// </summary>
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