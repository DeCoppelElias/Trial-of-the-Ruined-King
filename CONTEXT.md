# Trial of the Ruined King

A survival dodging game where the player dodges endless waves of escalating attacks on a grid arena, chasing high scores and gold.

This file is the glossary and nothing else: each word the project uses for its
own concepts, defined in a sentence or two, with the words to avoid. No
implementation, no decisions, no code. A general programming term does not
belong here.

## Language

### The run

**The Ruined King**:
The giant ruined figure overlooking the arena. He sends the attacks and comments on how the player plays.

**Arena**:
The grid of tiles where a run takes place.
_Avoid_: board, level

**Tile**:
One square of the arena. The player stands on exactly one tile at a time.
_Avoid_: cell, square

**Run**:
One attempt, from its start until the player dies or quits.
_Avoid_: game, round, session

**Difficulty**:
How hard the run is at the moment. It rises over the course of a run.

**Realm**:
A phase of a run with its own environment and set of attacks, unlocked by reaching a high enough score.
_Avoid_: stage, level, zone, biome

### Attacks

**Attack**:
One strike, such as a spear or a hammer, that moves through its stages and threatens a set of tiles.

**Stage**:
One step of an attack's life: Telegraph, Commit, Impact or Recovery. Stage never means a phase of a run; that is a realm.
_Avoid_: phase, state

**Telegraph**:
The first stage of an attack, where it shows itself coming and cannot hurt yet.

**Warning**:
Tiles marked red to show where an attack is about to hit.
_Avoid_: telegraph, highlight

**Danger tile**:
A tile that hurts the player right now.
_Avoid_: warning tile, red tile

**Pattern**:
The set of tiles an attack threatens, and how that set moves over its stages. The stage says when, the pattern says where.

**Wave**:
Whatever spawns each time the spawn timer fires: either a sequence or a burst of random attacks.
_Avoid_: round

**Sequence**:
A hand-authored wave: a timed list of attacks designed to be learned.
_Avoid_: combo

### Rewards

**Score**:
What a run earns, shown while it runs.
_Avoid_: points

**High score**:
The best score across all runs, kept between sessions.
_Avoid_: best, record

**Coin**:
A pickup on the arena during a run, worth gold.
_Avoid_: gold (for the pickup)

**Gold**:
The total collected across runs, spent outside a run. Any pickup can be worth gold, whatever it looks like.
_Avoid_: coins, money, currency
