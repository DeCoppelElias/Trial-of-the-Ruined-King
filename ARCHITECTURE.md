# Trial of the Ruined King architecture

This file maps the code as it is today: its assemblies, what each one holds,
and where the tests live. It is written for whoever changes the code. It never
holds why the code is shaped this way, which belongs in `docs/adr/`, or what
the game is for, which belongs in `DESIGN.md`. Update it in the same change
that moves, adds or removes an assembly or a port.

## Code layout

The code follows ports and adapters. `Assets/Scripts/Domain` is the game's
rules in plain C#. Each `Assets/Scripts/Adapter.*` folder is its own assembly
that depends on the domain and connects it to Unity. The domain still uses a
few Unity types; the [ports and adapters](docs/roadmap/ports-and-adapters.md)
entry settles that.

- `Domain`: models, services, and the interfaces in `Domain/Ports`.
- `Adapter.Bootstrap`: `GameBootstrap`, which builds and wires everything.
- `Adapter.Gameplay`: `DomainUpdater`, which ticks the domain, plus spawning
  and the attack factories.
- `Adapter.GamePresentation`: everything the player sees and hears of a run.
- `Adapter.Player`: player input and movement.
- `Adapter.UI`: menus, stats and the game over screen.
- `Adapter.Shared`: attack definitions used by gameplay and presentation.
- `Adapter.PlayerPrefsStorage`: implements `IStorage` with PlayerPrefs.
- `Adapter.UnityLogging`: implements `ILogger` with the Unity console.
- `Assets/Scripts/Editor`: inspector drawers.
- `Assets/Scripts/Other`: loose scene scripts outside any assembly, such as
  `FloatingMovement`.

## Tests

Tests live in `Assets/Tests`: domain tests in `Domain Tests`, and bug
verification tests in `Bug Verification`, split into EditMode, PlayMode and a
small shared assembly. How to run them is in `AGENTS.md`.
