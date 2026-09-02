using System;
using UnityEngine;

namespace Feature.UI.Adapter
{
    public class ElevationButton : ImageButton
    {
        [SerializeField] private GameObject _greyHover;
        public float CurrentOffsetHeight => transform.position.y;

        protected override void CheckForNull()
        {
            base.CheckForNull();
            if (_greyHover == null)
                throw new NullReferenceException("Button had null grey hover");
        }

        public void SetHeight(float height)
        {
            transform.position = new Vector3(transform.position.x, height, transform.position.z);
        }

        public void ShowGreyHover()
        {
            _greyHover.SetActive(true);
        }

        public void HideGreyHover()
        {
            _greyHover.SetActive(false);
        }
    }
}