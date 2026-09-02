using R3;

namespace Feature.UI.Adapter.Interfaces
{
    public interface IImageButton
    {
        public Observable<Unit> Click { get; }
        public Observable<Unit> Down { get; }
        public Observable<Unit> Up { get; }
    }
}