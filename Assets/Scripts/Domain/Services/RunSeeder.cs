using Domain.Models;
using Domain.Ports.Driven;
using Domain.Ports.Driving;

namespace Domain.Services
{
    // Gives each run its own seed. Build it before anything else subscribes to
    // OnStateChanged, so the reseed happens before any roll on run start.
    // See docs/adr/randomness.md.
    public class RunSeeder
    {
        private readonly RandomSource _runRandom;
        private readonly ILogger _logger;
        private readonly int? _fixedSeed;

        public RunSeeder(IGameStateService gameState, RandomSource runRandom, ILogger logger, int? fixedSeed = null)
        {
            _runRandom = runRandom;
            _logger = logger;
            _fixedSeed = fixedSeed;
            gameState.OnStateChanged += HandleStateChanged;
        }

        private void HandleStateChanged(GameState previous, GameState next)
        {
            if (next != GameState.Gameplay) return;

            int seed = _fixedSeed ?? RandomSource.NewSeed();
            _runRandom.Reseed(seed);
            _logger.LogInfo($"Run seed: {seed}");
        }
    }
}
