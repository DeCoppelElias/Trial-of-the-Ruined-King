# Trial of the Ruined King design

This file holds what Trial of the Ruined King is for: its vision, its goals and what it
will not do. It never holds how the code is built, which belongs in
`docs/adr/`, and it never links a decision or names a file in the code. It
changes when the direction changes, not when a decision does, and it stays
under 150 lines.

## Vision

A game about dodging endless waves of attacks sent by the Ruined King, chasing
high scores and collecting gold. The player gets better by learning how the
attacks and the game's other systems work. The King comments on how you play,
and that gives the game its depth.

The player should feel like a ninja: reading patterns, dodging cleanly and
improving run by run while pushing a personal best. The feel is exciting,
engaging and about learning.

### Touchstones

These games show how to make this kind of game fun.

- Subway Surfers: unlockable characters and cosmetics that bring players back,
  quick runs and instant restarts.
- Jetpack Joyride: missions during a run that pay out currency, and runs that
  keep surprising you.
- Temple Run: one more try. Runs are short, a death is your own fault, and the
  best score pulls you back.

## Goals

1. Every death is the player's fault. Every attack is telegraphed long enough
   to read, and there is always a way out.
2. Skill comes from learning the game's systems: the attacks today, and later
   things like tile modifiers or obstacles.
3. Runs stay fresh. New patterns and milestones arrive before the player gets
   bored.
4. The Ruined King comments on what happens in a run, such as streaks, near
   misses and deaths. His tone starts cold and warms up across runs, the more
   the player keeps trying.
5. Mobile is the main target. The demo ships on PC to test with players
   whether the game is fun, then the game moves to mobile and keeps growing
   there. The controls have to carry over to touch.

## Non-goals

- Fighting back. The player only moves and dodges.
- Randomness deciding a death. What spawns can vary, but an attack is never
  unreadable and the arena is never sealed off.
- Multiplayer.
- Grinding, or large unlock trees.
- Loot or inventory systems.
