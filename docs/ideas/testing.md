# Testing

Status: idea.

## What it does

Decides how the game is tested: what runs in EditMode against the plain
domain, what needs PlayMode, what a roadmap entry must test before it counts
as done, and how the `KnownBug` and `DesignQuestion` categories are used.

## Why it is interesting

Today there are three test assemblies: domain tests, and EditMode and PlayMode
bug verification. Most game logic outside the domain, such as spawning, can
only be tested in PlayMode. How much can move to EditMode depends on
[ports and adapters](ports-and-adapters.md), so grill this right after it.

## What it would touch

The test assemblies and folders, and the definition of done in `AGENTS.md`.

## Open questions

- What must have a test, and what is fine untested, such as presentation?
- Do bug verification tests stay a separate group once their bugs are fixed,
  or become ordinary tests?
- Is there a target for how fast the EditMode suite runs?
