# Ports and adapters

The code is hexagonal. `Domain` holds the game's rules in plain C# and sets
`noEngineReferences`, so the compiler rejects any Unity type in it. The domain
reaches the outside only through ports, in two folders with matching
namespaces. `Domain/Ports/Driving` holds the domain's API, implemented by its
services and called by adapters. `Domain/Ports/Driven` holds what the domain
needs from outside, implemented by adapters.

Each adapter is its own assembly and references only the domain, Unity
packages and `Shared`. Adapters never reference each other, and when two must
coordinate they do it through the domain. `Bootstrap` is the composition root
and the one assembly that references everything. `Shared` is a library, not an
adapter: it references only the domain and holds only Unity types that more
than one adapter uses. Adapters depend on the driving port interfaces and on
domain models, never on concrete services. Only `Bootstrap` names a service.

A Unity-free domain is what lets its rules run in EditMode tests without a
scene, and a rule the compiler checks does not erode. Banning references
between adapters keeps each one replaceable and exposes game rules that would
otherwise hide in a presenter.

A simple driven port, such as storage or logging, follows this decision and
needs no file of its own. A port gets its own decision when choosing it is
hard to reverse, such as a save format or an online service.

Unity stores the assembly and namespace of every `[SerializeReference]` type
inside the asset, so renaming the `Gameplay` assembly or moving its sequence
step parameters to a namespace empties those steps unless the assets are
rewritten too. That is why the assembly names do not carry an `Adapter.`
prefix, and why adapters stay in the global namespace while the domain has
namespaces that follow its folders.
