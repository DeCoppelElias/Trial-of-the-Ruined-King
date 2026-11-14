# Attack lifecycle

Status: idea.

## What it does

Decides how an attack lives: a stage machine through Telegraph, Commit,
Impact and Recovery, with the pattern kept separate. The stage says when, the
pattern says where.

## Why it is interesting

Three ideas push on it. [Fairness guarantees](fairness-guarantees.md) wants
the warning in the domain, perhaps as its own stage.
[Frame rate independence](frame-rate-independence.md) changes how a stage
hands leftover time to the next (N1).
[Hitbox or graphic](hitbox-or-graphic.md) asks what leads during Impact.
Settling the lifecycle once keeps those grills small. Grill it after the
[game clock](game-clock.md), since stages run on it.

## What it would touch

The attack model, attack timing, the patterns, and the attack presentations.

## Open questions

- Is the warning a stage, or data on Telegraph?
- Does a stage carry leftover time into the next?
- Are the stages fixed for every attack, or can an attack skip or repeat one?
