# Frame rate independence

Status: idea.

## What it does

Makes attacks last exactly their configured time at any frame rate, by
carrying leftover time into the next stage (N1). Fast spears would also check
every tile they crossed since the last tick (B7b).

## Why it is interesting

Mobile is the main target, and phones vary in frame rate far more than a PC
does. Today a 1.40 second spear lasts 1.47 seconds at 30 fps. See the
[attack system review](../attack-system-review.md), topic 8.

## What it would touch

The attack stage timing, and the spear pattern.

## Open questions

- It shifts every attack's timing slightly. Retune afterwards, or accept it?
