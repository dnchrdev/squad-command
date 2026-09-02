using System.Collections.Generic;
using System.Linq;
using Feature.Command.Application.Interfaces;
using Feature.Command.Domain.Data;
using Feature.Command.Infrastructure.Data;
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