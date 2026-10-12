using System.Collections.Generic;
using Features.GameLifecycle.UseCases;
using NUnit.Framework;
using Tests.EditMode.GameLifecycle.UseCases.Fakes;

namespace Tests.EditMode.GameLifecycle.UseCases
{
    public class GameStartUseCaseTests
    {
        private const string CAMERA = "camera";
        private const string COMMAND = "command";

        [Test]
        public void Execute_StartsCameraThenCommand()
        {
            var log = new List<string>();
            var useCase = new GameStartUseCase(new SpyStart(CAMERA, log), new SpyStart(COMMAND, log));

            useCase.Execute();

            Assert.That(log, Is.EqualTo(new[] { CAMERA, COMMAND }));
        }
    }
}
