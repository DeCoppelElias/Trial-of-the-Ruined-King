# Fairness guarantees

Status: idea.

## What it does

Turns the fairness goal into rules the game enforces in one place, for example
a minimum number of free tiles, a safe tile always within reach, and a minimum
warning time.

## Why it is interesting

Today the warning is 25% of the spear telegraph and 10% of the hammer
telegraph, about 0.125 seconds for both at maximum difficulty. Random attacks
can also cover the whole arena (B6b), and tile tracking forgets shared tiles
(B6). Warning timing is a gameplay rule hardcoded in the presentation (A3).
See the [attack system review](../attack-system-review.md), topic 3.

## What it would touch

The attack factories, attack timing, tile tracking, and the warning
presentation.

## Open questions

- Which guarantees, and what numbers?
- Do the guarantees hold at every difficulty, or relax at the top?
