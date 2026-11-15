# Grid arena

Status: planned, see the [roadmap entry](../roadmap/grid-arena.md).

## What it does

Records how the code models the arena: discrete tiles, step movement from tile
to tile, and hits checked per tile instead of with physics colliders.

## Why it is interesting

The design already puts the player on exactly one tile at a time, so this grill
will mostly confirm the code and record why there is no physics. That is hard
to reverse and surprises a reader. [Hitbox or graphic](hitbox-or-graphic.md)
builds on it.

Mapping tiles to world space is not part of the domain arena. It lives in
`ArenaSpace` in presentation, per the
[ports and adapters](../adr/ports-and-adapters.md) decision.

## What it would touch

`RectangleArena`, player movement, and hit checks.

## Open questions

- Is the arena always a rectangle, or do realms need other shapes or blocked
  tiles?
- Does a step move happen at once in the domain, with the graphic catching
  up?
