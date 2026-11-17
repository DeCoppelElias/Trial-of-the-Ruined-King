# Spawning in the domain

Status: not started.

Builds the [spawning in the domain](../ideas/domain-spawning.md) idea. Its
grill settles the `domain-spawning` decision, then the code changes to follow
it fully. It depends on the [game clock](game-clock.md) and the [game over
state](game-over-state.md). It is bound by the decisions of every entry it
depends on.

The [randomness](../adr/randomness.md) decision binds it too. Spawning draws
every roll from `runRandom` in an order fixed by the game clock, and this entry
adds the test that two runs with the same seed and inputs play the same.
