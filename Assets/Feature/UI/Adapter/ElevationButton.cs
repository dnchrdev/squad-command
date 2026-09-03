using UnityEngine;

namespace Feature.UI.Adapter
{
    public class ElevationButton : ImageButton
    {
        [SerializeField] private GameObject _greyHover;
        public float CurrentOffsetHeight => transform.position.y;

        protected override void Initialize()
        {
            base.Initialize();
            HideGreyCover();
        }

        public void SetHeight(float height)
        {
            transform.position = new Vector3(transform.position.x, height, transform.position.z);
        }

        public void ShowGreyCover()
        {
            SetGreyCoverVisibility(true);
        }

        public void HideGreyCover()
        {
            SetGreyCoverVisibility(false);
        }
        private void SetGreyCoverVisibility(bool value)
        {
            if (_greyHover == null) return;
            _greyHover.SetActive(value);
        }
    }
}