using System.Collections.Generic;
using Features.Core.Infrastructure.Interfaces;

namespace Tests.EditMode.GameplayECS.MoveCommand.Fakes
{
    public sealed class FakeObjectPool<T> : IObjectPool<T>
    {
        private readonly Queue<T> _free = new();

        public readonly List<T> Returned = new();

        public int FreeCount => _free.Count;

        public bool TryGet(out T item) => _free.TryDequeue(out item);

        public void Return(T item)
        {
            Returned.Add(item);
            _free.Enqueue(item);
        }

        public void Prewarm(T item) => _free.Enqueue(item);
    }
}
