# Sword sweep

Status: idea.

## What it does

A new attack. A sword appears beside one edge of the arena and sweeps across
three tiles of that edge very fast. In the telegraph the warnings light one by
one in sweep order, so the player reads the direction. In the commit the blade
winds back and glints. In the impact it crosses the three tiles in about 130 ms,
and each tile is a danger tile only while the blade is over it. In the recovery
it follows through and fades.

The [Attack Animation Lab](https://claude.ai/artifact/PVdV7Yqd2TVW1CUGrLUZf9)
prototypes it with adjustable stage durations.

## Why it is interesting

It is faster than any current attack and its pattern moves through a single
stage, which asks a different read from the player than the spear or the
hammer.

## What it would touch

A new attack, its pattern, factory, definition and presentation. The effects
come from [attack effects](attack-effects.md).

## Open questions

- At default timing each tile is dangerous for about 30 ms, under two frames at
  60 fps. The hit check must cover the time since the last tick, the same
  question as in [hitbox or graphic](hitbox-or-graphic.md).
- Are the danger windows computed from the swing curve, or authored per tile?
- Which edges and which three tiles can it hit, and can it appear in a sequence?
- Which sword model, from the LowPolyRPGWeapons pack or a new one?
- Builds on [attack extensibility](attack-extensibility.md), so adding it touches fewer places.
