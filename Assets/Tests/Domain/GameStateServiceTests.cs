using Domain.Models;
using Domain.Services;
using NUnit.Framework;

namespace Domain.Tests
{
    [TestFixture]
    public class GameStateServiceTests
    {
        [Test]
        public void StartsInMainMenu()
        {
            Assert.AreEqual(GameState.MainMenu, new GameStateService().CurrentState);
        }

        [Test]
        public void TransitionTo_ChangesState_AndReportsPreviousAndNew()
        {
            var service = new GameStateService();
            GameState? previous = null, next = null;
            service.OnStateChanged += (p, n) => { previous = p; next = n; };

            service.TransitionTo(GameState.Gameplay);

            Assert.AreEqual(GameState.Gameplay, service.CurrentState);
            Assert.AreEqual(GameState.MainMenu, previous);
            Assert.AreEqual(GameState.Gameplay, next);
        }

        [Test]
        public void TransitionTo_CurrentState_DoesNothing()
        {
            var service = new GameStateService();
            int changes = 0;
            service.OnStateChanged += (p, n) => changes++;

            service.TransitionTo(GameState.MainMenu);

            Assert.AreEqual(0, changes);
        }
    }
}
