# Difficulty curve

Status: idea.

## What it does

Reshapes how difficulty rises. Today it ramps linearly over 300 seconds on the
spawn interval and attack timing. Difficulty tiers that choose which sequences
can appear are one option.

## Why it is interesting

The original vision asks for a ramp that starts forgiving and becomes intense
without feeling unfair. Today sequences ignore difficulty apart from timing,
`difficultyRating` is unused, and at the 0.7 second minimum interval a new
wave starts before longer sequences finish, cutting them off. See the
[attack system review](../attack-system-review.md), topic 6.

## What it would touch

Spawning, the sequence definitions, and attack timing.

## Open questions

- Should a wave always play to its end?
- Linear, stepped, or tied to [realms](realms.md)?
