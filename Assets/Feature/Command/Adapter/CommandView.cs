using System.Collections.Generic;
using System.Linq;
using Feature.Command.Application.Interfaces;
using Feature.Command.Domain.Data;
using Feature.Command.Infrastructure.Data;
using Feature.Shared.DevTools;
using Feature.UI.Adapter;
using R3;
using UnityEngine;

namespace Feature.Command.Adapter
{
    public class CommandView : MonoBehaviour, ICommandView
    {
        [SerializeField] private List<CommandButtonEntry> _buttons;
        [SerializeField] private Transform _lowHeight;
        [SerializeField] private Transform _highHeight;

        private Dictionary<CommandType, ElevationButton> _buttonMap =  new Dictionary<CommandType, ElevationButton>();

        private void OnValidate()
        {
            if(_buttons.Count == 0) 
                Debug.LogError($"_buttons.Count == 0");
            
            InspectorRefValidator.CheckAssigned(_lowHeight, nameof(_lowHeight), this);
            InspectorRefValidator.CheckAssigned(_highHeight, nameof(_highHeight), this);
        }
        
        public void Initialize()
        {
            _buttonMap = _buttons.ToDictionary(e => e.Type, e => e.Button);
        }
        
        public Observable<Unit> GetClick(CommandType type) => _buttonMap[type].Click;
        public float GetLowHeight() => _lowHeight.position.y;
        public float GetHighHeight() => _highHeight.position.y;

        public float GetButtonHeight(CommandType type) => _buttonMap[type].CurrentOffsetHeight;
        public void SetButtonHeightOffset(CommandType type, float height) => _buttonMap[type].SetHeight(height);
    }
}