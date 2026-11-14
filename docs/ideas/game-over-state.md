# Game over state

Status: idea.

## What it does

Adds a state for when the player has died. Today the game only knows the main
menu and gameplay, so death is purely visual and the run keeps simulating. A
game over state would stop hits, scoring and spawning the moment the player
dies.

## Why it is interesting

It fixes B1 (every hit after death triggers death again), B2 (score keeps
rising after death) and most of B5 at the root, instead of patching each one.
See the [attack system review](../attack-system-review.md), topic 1.

## What it would touch

The game states, the player's death, scoring, saving the high score, spawning,
and the death presentation.

## Open questions

- Does the death slow motion run inside the game over state or before it?
- Is the score frozen at the moment of death?
- Built on the [game state](game-state.md) model, grilled first.
