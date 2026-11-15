# Game over state

Status: not started.

Builds the [game over state](../ideas/game-over-state.md) idea. Its grill
settles the `game-over-state` decision, then the code changes to follow it
fully. It depends on the [game state](game-state.md). It is bound by the
decisions of every entry it depends on. It replaces the bridge in `Bootstrap` from
`DeathPresenter` to `ShowGameOver`.
