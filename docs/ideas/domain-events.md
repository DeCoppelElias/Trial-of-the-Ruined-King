# Domain events

Status: idea.

## What it does

Decides how the domain tells the outside what happened. Today it raises plain
C# events, and presentation only listens.

## Why it is interesting

It seemed sensible and easy to extend, but it was not thought through. Plain
events leave open who unsubscribes, in what order listeners run, and what
happens when a listener changes the domain mid-event. New effects, sounds and
King commentary will all hang off these events. Grill it after
[ports and adapters](ports-and-adapters.md).

## What it would touch

Every domain service and model that raises events, and every presenter.

## Open questions

- Plain C# events, an event bus, or presenters polling domain state each
  frame?
- May a listener act on the domain during an event, or only record it?
- Who unsubscribes, and when?
