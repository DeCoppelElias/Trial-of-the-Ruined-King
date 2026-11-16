# Game clock

Status: built, see the [game clock](../adr/game-clock.md) decision.

## What it does

Decides who owns time in a run: how the domain advances, how slow motion, hit
stop and pausing work, and which clock each adapter must use.

## Why it is interesting

The domain advances only on ticks that `DomainUpdater` hands it, scaled by its
own factor; `Time.timeScale` is never set. Adapters each pick a clock, and
several pick wrong. Spawning runs on Unity time (B5). Hit stop pauses the
domain but leaves the scale at 1, so the hammer Animator and the player
animations, which follow the scale, keep moving through it, and hammer impact
particles follow no game clock at all. One pause flag serves every pauser
(N2). See the
[attack system review](../attack-system-review.md).

[Spawning in the domain](domain-spawning.md),
[frame rate independence](frame-rate-independence.md) and the
[attack lifecycle](attack-lifecycle.md) all build on this choice, so grill it
before them.

Until this is built, presenters read and set the time scale through delegates
that `Bootstrap` wires to `DomainUpdater`, because the
[ports and adapters](../adr/ports-and-adapters.md) decision bars presentation
from referencing `Gameplay`. This entry replaces those delegates.

## What it would touch

`TimeService`, `DomainUpdater`, the hit and death presentations, and every
adapter that times anything.

## Open questions

- Does the domain only advance on ticks it is handed, or read Unity time? Ticks
  keep it testable in EditMode and allow seeded runs.
- Where do slow motion and hit stop live: `Time.timeScale`, which Animator,
  particles and `WaitForSeconds` follow for free, or our own scale, which each
  adapter must opt into (a game time wait, `animator.speed`, particle
  `simulationSpeed`)?
- How do several pausers share the clock (N2): a counter, a set of owners, or
  something else?
- A fixed timestep inside the domain, or variable steps?
