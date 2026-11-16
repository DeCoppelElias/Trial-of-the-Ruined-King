# Game state

Status: planned, see the [roadmap entry](../roadmap/game-state.md).

## What it does

Decides the model for the states the game moves through, which states exist,
who may change them, and how a state relates to pausing the clock. Today there
are only `MainMenu` and `Gameplay`.

## Why it is interesting

The [game over state](game-over-state.md) adds one state, and a pause menu,
the tutorial and realms will add more. Without a model each one wires itself
into `GameStateService` and the clock differently. Grill it after the
[game clock](game-clock.md); game over is then built on it.

## What it would touch

`GameStateService`, everything that listens to state changes, and the clock.

## Open questions

- A flat list of states, or nested ones, such as paused inside gameplay?
- Who may request a change, and can a change be refused?
- Do moves made during a pause count, are they dropped, or are they held until
  the pause ends? Today a move still reaches the domain during hit stop.
