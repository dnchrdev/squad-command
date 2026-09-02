using Vector2 = UnityEngine.Vector2;

namespace Feature.Command.Application.Interfaces
{
    public interface IMoveVisualView
    {
        public void Show();
        public void Hide();

        void Initialize();
        
        public Vector2 GetScreenScaleFactor();

        public void SetSelectionStart(Vector2 position);
        public void UpdateSelectionEnd(Vector2 position);
        public void SetLinkerPosition(Vector2 position);
        public void SetLinkerZRotation(float zRotation);
        public void SetLinkerLength(float length);
    }
}