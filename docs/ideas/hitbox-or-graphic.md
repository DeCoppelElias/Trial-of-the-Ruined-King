# Hitbox or graphic

Status: idea.

## What it does

Decides which one leads for the spear: either the danger tile follows the
spear's real position, or the spear's path is fitted to the danger timing.
Either way there is one source of truth for where the spear is (A4).

## Why it is interesting

Today the hitbox runs ahead of the graphic, so players get hit by a spear that
has visibly not reached them (B7). That breaks the fairness goal directly. See
the [attack system review](../attack-system-review.md), topic 4.

## What it would touch

The spear pattern, the spear presentation, and hit checks.

## Open questions

- Should a fast attack check every tile it crossed since the last tick (B7b)?
