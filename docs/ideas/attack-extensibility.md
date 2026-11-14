# Attack extensibility

Status: planned, see the [roadmap entry](../roadmap/attack-extensibility.md).

## What it does

Makes adding an attack touch a few places instead of about ten across three
assemblies. Each definition asset would create its own gameplay and
presentation factories, one list of definitions would feed both spawning and
presentation, and lookups by C# type would go. It also decides the content
format: how an attack and a sequence are described as assets.

## Why it is interesting

The original vision asks for 3 to 5 distinct attack patterns in the demo, and
today an attack missing from one list is invisible but still deals damage. The
review recommends doing this before adding a fourth attack. It covers A2 and
A5. See the [attack system review](../attack-system-review.md), topic 7.

## What it would touch

The attack factories, the sequence definitions, the presentation factories,
and the scene setup.

## Open questions

- Do this before or after [spawning moves into the domain](domain-spawning.md)?
- Keep attacks and sequences as ScriptableObject assets? Keep sequence steps in `[SerializeReference]`? This grill ends in an `attack-content` decision.
