namespace Feature.Core.Infrastructure.Interfaces
{
    public interface IObjectPool<T>
    {
        int FreeCount { get; }
        bool TryGet(out T item);
        void Return(T item);
        void Prewarm(T item);
    }
}