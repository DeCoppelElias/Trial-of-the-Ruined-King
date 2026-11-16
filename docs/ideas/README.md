# Ideas

One file per idea that is wanted and not decided, named by the idea. Each has
a status line, then the sections What it does, Why it is interesting, What it
would touch, and Open questions. Nothing here is committed to.

The status is `idea`. Once an idea is adopted it reads `planned`, with a link
to its roadmap entry, and once built it reads `built`, with a link to the
decision that settled it. The file stays either way.

Each idea gets one line below, a link and a few words.

The game:

- [King commentary](king-commentary.md): the Ruined King comments on your play and warms up across runs.
- [Realms](realms.md): environments with their own attacks, reached and unlocked by score.
- [Gold spending](gold-spending.md): what gold buys.
- [Playable characters](playable-characters.md): characters to unlock.
- [Leaderboards](leaderboards.md): comparing scores, locally then online.
- [Challenges](challenges.md): goals within a run that pay gold.
- [Achievements](achievements.md): milestones across runs.
- [Challenge modes](challenge-modes.md): seeded runs and other rulesets.
- [Accessibility](accessibility.md): options for reading attacks.
- [Tutorial and end card](tutorial-and-end-card.md): the start and end of the demo loop.
- [Demo scope](demo-scope.md): what the PC demo must prove, with the original vision document.
- [Attack effects](attack-effects.md): trails, glows, particles and decals that sell each stage.
- [Sword sweep](sword-sweep.md): a fast sword that sweeps three edge tiles.

From the attack system review:

- [Game over state](game-over-state.md): stop the run when the player dies.
- [Spawning in the domain](domain-spawning.md): one clock for waves and attacks.
- [Fairness guarantees](fairness-guarantees.md): free tiles and warning times the game enforces.
- [Hitbox or graphic](hitbox-or-graphic.md): which one leads for the spear.
- [Scoring](scoring.md): how a run earns its score.
- [Difficulty curve](difficulty-curve.md): how difficulty rises.
- [Attack extensibility](attack-extensibility.md): fewer places to touch when adding an attack.
- [Frame rate independence](frame-rate-independence.md): same timing at any frame rate.

Architecture topics, each grilled on its own to settle a decision and bring the
code in line. The [roadmap](../roadmap/README.md) holds the order.

- [Version control](version-control.md): Git or Plastic as the source of truth.
- [Ports and adapters](ports-and-adapters.md): the domain core and its boundary to Unity.
- [Testing](testing.md): what is tested where, and what done requires.
- [Game clock](game-clock.md): who owns time, scaling and pausing.
- [Randomness](randomness.md): one seedable source of random rolls.
- [Domain events](domain-events.md): how the domain reports what happened.
- [Composition root](composition-root.md): how the game is wired at start.
- [Game state](game-state.md): the states a run moves through.
- [Grid arena](grid-arena.md): tiles, step movement and per-tile hits.
- [Attack lifecycle](attack-lifecycle.md): stages and patterns of an attack.
- [Application layer](application-layer.md): use cases split from the domain model, once services orchestrate.

Tooling:

- [Unity MCP](unity-mcp.md): an agent drives the open editor, scenes and screenshots.
