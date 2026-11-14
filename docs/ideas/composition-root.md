# Composition root

Status: idea.

## What it does

Decides how the game is wired together at start. Today `GameBootstrap` builds
every service by hand and calls `Initialize` on each MonoBehaviour.

## Why it is interesting

The hand wiring was not a choice; a dependency injection framework was never
looked into. Every new service adds lines to one class, and a MonoBehaviour
used before its `Initialize` call fails quietly. Grill it after
[ports and adapters](ports-and-adapters.md) and
[domain events](domain-events.md), since it wires what they define.

## What it would touch

`GameBootstrap`, every MonoBehaviour with an `Initialize` method, and the
scene.

## Open questions

- Hand wiring, VContainer, or Zenject (Extenject)?
- How do scene objects and spawned prefabs get their dependencies?
