# Spawning in the domain

Status: idea.

## What it does

Moves wave and sequence scheduling out of Unity coroutines and into a domain
service driven by the game's own clock. Attacks already run on that clock;
spawning does not.

## Why it is interesting

It fixes B3 (wave coroutines cannot be stopped) and B5 (spawning runs on Unity
time) together, makes the spawner testable without PlayMode, and gives one
clock for everything. It also covers splitting `PlayerAttacker` (A1). See the
[attack system review](../attack-system-review.md), topic 2.

## What it would touch

`PlayerAttacker`, the domain services, the sequence definitions, and the tests.

## Open questions

- Does a new wave cancel the one still playing? Today it tries to and fails
  (B3). This ties into the [difficulty curve](difficulty-curve.md).
