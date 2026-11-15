# Trial of the Ruined King architecture

This file maps the code as it is today: its assemblies, what each one holds,
and where the tests live. It is written for whoever changes the code. It never
holds why the code is shaped this way, which belongs in `docs/adr/`, or what
the game is for, which belongs in `DESIGN.md`. Update it in the same change
that moves, adds or removes an assembly or a port.

## Code layout

The code follows the [ports and adapters](docs/adr/ports-and-adapters.md)
decision. `Assets/Scripts/Domain` is the game's rules in plain C# with no Unity
references. Each `Assets/Scripts/Adapter.*` folder is its own assembly that
depends on the domain and connects it to Unity. Adapters reference only the
domain, Unity packages and `Shared`, except `Bootstrap`, which references
everything.

- `Domain`: one namespace per layer.
  - `Domain/Models` (`Domain.Models`): the arena, attacks, player, coin and
    game state.
  - `Domain/Services` (`Domain.Services`): the services behind the driving
    ports.
  - `Domain/Ports/Driving` (`Domain.Ports.Driving`): the domain's API, which
    adapters call. The services implement it.
  - `Domain/Ports/Driven` (`Domain.Ports.Driven`): `IStorage` and `ILogger`,
    which adapters implement.
- `Adapter.Bootstrap`: `GameBootstrap`, which builds and wires everything. It
  also bridges presentation to `DomainUpdater` for the time scale and to the UI
  for the game over screen.
- `Adapter.Gameplay`: `DomainUpdater`, which ticks the domain, plus spawning
  and the attack factories.
- `Adapter.GamePresentation`: everything the player sees and hears of a run,
  including `ArenaSpace`, which maps tiles to world positions.
- `Adapter.Player`: player input and movement.
- `Adapter.UI`: menus, stats and the game over screen.
- `Adapter.PlayerPrefsStorage`: implements `IStorage` with PlayerPrefs.
- `Adapter.UnityLogging`: implements `ILogger` with the Unity console.
- `Shared`: a library, not an adapter. It holds the attack definitions used by
  gameplay and presentation.
- `Assets/Scripts/Editor`: inspector drawers.

The assembly names drop the `Adapter.` prefix of their folders, so `Gameplay`,
`GamePresentation` and so on.

## Tests

Tests live in `Assets/Tests`, one folder and assembly per assembly under test,
as the [testing](docs/adr/testing.md) decision sets out. How to run them is in
`AGENTS.md`.

- `Domain` (`Domain.Tests`): EditMode tests of the domain models and services.
  `Fakes` holds in-memory stand-ins for the driven ports.
- `GamePresentation` (`GamePresentation.Tests`): EditMode tests of `ArenaSpace`.
- `Gameplay` (`Gameplay.Tests`): an EditMode test that validates every attack
  sequence asset.
- `PlayMode` (`PlayMode.Tests`): the smoke test, which loads `SampleScene` and
  starts a run.
