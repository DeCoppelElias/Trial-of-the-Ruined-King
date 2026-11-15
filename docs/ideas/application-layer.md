# Application layer

Status: idea.

## What it does

Splits the domain assembly in two: a model of entities and rules, such as
`Attack`, `RectangleArena` and `Player`, and an application layer of use cases
that orchestrate the model and call the driven ports, such as starting a run or
saving stats on exit.

## Why it is interesting

Today the services are mostly rules themselves, and only
`PlayerStatsPersistence` is plainly orchestration, so the
[ports and adapters](../adr/ports-and-adapters.md) decision keeps one domain
assembly. The split becomes worth it when services start coordinating several
driven ports, as saving, [leaderboards](leaderboards.md) and
[achievements](achievements.md) would, or when a use case fits awkwardly in
any one service. Raise it again when either happens.

## What it would touch

The domain assembly, `Domain/Services`, the domain tests, and `Bootstrap`.

## Open questions

- Which services are rules and which are use cases?
- Do the driving ports belong to the application layer once it exists?
