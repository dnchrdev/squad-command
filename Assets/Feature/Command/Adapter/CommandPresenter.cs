using System;
using Feature.Command.Application;
using Feature.Command.Domain;
using Feature.Command.Domain.Data;
using Feature.Input.Infrastructure.Interfaces;
using R3;
using UnityEngine;
using Zenject;

namespace Feature.Command.Adapter
{
    public sealed class CommandPresenter : IInitializable, IDisposable
    {
        [Inject] private readonly IReadOnlyCommandInput _input;
        [Inject] private readonly CommandState _commandState;

        private readonly CompositeDisposable _disposables = new();
        private Vector2 _currentPointer;

        public void Initialize()
        {
            _input.PointerPosition
                .Subscribe(pos => _currentPointer = pos)
                .AddTo(_disposables);
            
            _input.PointerPosition
                .Subscribe(pos => _commandState.Current.OnPointerMove(pos))
                .AddTo(_disposables);

            _input.ClickDown
                .Subscribe(_ => _commandState.Current.OnPointerDown(_currentPointer))
                .AddTo(_disposables);

            _input.ClickUp
                .Subscribe(_ => _commandState.Current.OnPointerUp(_currentPointer))
                .AddTo(_disposables);

            _input.Select.Subscribe(_ => _commandState.SetSelect()).AddTo(_disposables);
            _input.Move.Subscribe(_ => _commandState.SetMove()).AddTo(_disposables);
            //_input.Attack.Subscribe(_ => _commandState.SetAttack()).AddTo(_disposables);
        }

        public void Dispose() => _disposables.Dispose();
    }
}