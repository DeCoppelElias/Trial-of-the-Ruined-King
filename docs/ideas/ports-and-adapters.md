# Ports and adapters

Status: planned, see the [roadmap entry](../roadmap/ports-and-adapters.md).

## What it does

Decides the shape of the code, also called hexagonal architecture: a domain
core that knows nothing of Unity, reaching the outside only through ports, with
each adapter in its own assembly. See Alistair Cockburn's
[Hexagonal Architecture](https://alistair.cockburn.us/hexagonal-architecture/)
for the pattern. The [architecture map](../../ARCHITECTURE.md) shows the assemblies and ports
as they are today.

## Why it is interesting

It was meant this way, but it may not have been done correctly. The domain
assembly still references `UnityEngine` and uses `Vector3`, `Mathf` and
`UnityEngine.Random`. Most interfaces in `Domain/Ports` are implemented by the
domain itself, so they describe services, not ports to the outside; only
storage and logging have adapters behind them. Testing, the composition root
and domain events all build on this, so grill it before them.

## What it would touch

Every assembly definition, the domain models that use Unity types, and the
`Domain/Ports` folder.

## Open questions

- May the domain use any Unity types, such as `Vector3` or `Mathf`? If not,
  should its assembly set `noEngineReferences`?
- Which interfaces are real ports (the domain calls out) and which are the
  domain's own service interfaces (adapters call in)? Do both live in `Ports`?
- One assembly per adapter, or fewer? Where does `Adapter.Shared` belong?
- Cheap ports like storage and logging follow the rule without their own
  decision. Is that enough?
