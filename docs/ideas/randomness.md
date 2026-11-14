# Randomness

Status: idea.

## What it does

Gives the game one injected, seedable source of randomness, in place of direct
calls to `UnityEngine.Random`.

## Why it is interesting

`UnityEngine.Random` is called inside the domain (`CoinService`,
`RectangleArena`) and in the attack factories, `PlayerAttacker` and several
presentations. A seeded run, which [challenge modes](challenge-modes.md) want,
needs every gameplay roll to come from one seed, while cosmetic rolls such as
shake must not consume it. Grill it after the [game clock](game-clock.md),
since a repeatable run needs both.

## What it would touch

The domain services and models that roll, the attack factories, spawning, and
the composition root.

## Open questions

- One source for gameplay and a separate one for cosmetics?
- Is it a port the domain asks for, or a plain class the domain owns?
