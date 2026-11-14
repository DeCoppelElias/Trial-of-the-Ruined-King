# Trial-of-the-Ruined-King
Trial of the Ruined King is a survival dodging game where the player has to dodge endless waves of escalating attacks on a grid arena. Chase high scores and collect gold in this mysterious survival game.

## Code layout

The code follows ports and adapters. `Assets/Scripts/Domain` is the game's
rules in plain C#. Each `Assets/Scripts/Adapter.*` folder is its own assembly
that depends on the domain and connects it to Unity.

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

Tests live in `Assets/Tests`: domain tests, and bug verification tests in
EditMode and PlayMode. How to run them is in `AGENTS.md`.
