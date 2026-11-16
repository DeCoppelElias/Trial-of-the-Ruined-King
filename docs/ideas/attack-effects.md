# Attack effects

Status: idea.

## What it does

Gives every attack effects that sell its stages. A swoosh trail behind fast
weapons. Warnings that pulse and danger tiles that glow. A glint on the weapon
at the end of the telegraph. Dust and a shockwave ring when the hammer lands.
Slash marks and cracks that fade from the tiles after impact. A short screen
flash on impact, on top of the arena shake and hit stop the game has already.

Weapons stay low-poly models, from the asset packs first and built only when no
pack model fits. The effects are code, shaders, particles and decals, not
hand-made art.

A browser prototype shows the look for the spears, the hammer and a
[sword sweep](sword-sweep.md):
[Attack Animation Lab](https://claude.ai/artifact/PVdV7Yqd2TVW1CUGrLUZf9).

## Why it is interesting

How an attack feels comes mostly from motion and what lights up when it lands,
less from the model. Clear stages also help the fairness goal: a glint or a
warning that lights in sweep order tells the player when and where before the
attack can hurt.

## What it would touch

Each attack's presentation, the tile visuals, materials and shaders, particle
systems, and camera effects. The spear already has a trail renderer.

## Open questions

- Which render pipeline features can the effects use, such as decals and bloom?
- Does each effect follow game time, so it slows and pauses with the run? See the [game clock](game-clock.md).
- Should screen flash and shake respect an [accessibility](accessibility.md) setting?
- Does the hammer keep its Animator clips, or move to code driven curves like the prototype?
