using System.Collections.Generic;
using Feature.Core.Infrastructure.Interfaces;

namespace Feature.Core.Infrastructure
{
    /// <summary>
    /// Простейшая реализация IObjectPool поверх Stack.
    /// Не владеет жизненным циклом T: не создаёт, не уничтожает, не активирует/деактивирует.
    /// </summary>
    public class ObjectPool<T> : IObjectPool<T>
    {
        private readonly Stack<T> _free;
 
        public ObjectPool(int capacity = 16)
        {
            _free = new Stack<T>(capacity);
        }
 
        public int FreeCount => _free.Count;
 
        public bool TryGet(out T item)
        {
            if (_free.Count > 0)
            {
                item = _free.Pop();
                return true;
            }
 
            item = default;
            return false;
        }
 
        public void Return(T item)
        {
            _free.Push(item);
        }
 
        public void Prewarm(T item)
        {
            _free.Push(item);
        }
    }
}