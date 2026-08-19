using Feature.GameplayECS.CommandProcessing;
using UnityEngine;
using Zenject;

namespace Feature.Command.Application
{
    public class MoveSquadUseCase
    {
        [Inject] private readonly ICommandSquadFacade _gameplay;

        public void MoveSelectedTo(Vector3 destination)
        {
            _gameplay.MoveSelectedTo(destination);
        }
    }
}