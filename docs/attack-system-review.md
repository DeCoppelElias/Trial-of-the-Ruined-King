# Attack system review

Review of the attack system (spawning, attack lifecycle, hit detection, presentation) as of commit `eb5eaec`, 2026-11-10. Nothing here is fixed yet. Each finding has an ID so it can be picked up later.

## How findings were verified

Reproduction tests live in `Assets/Tests/Bug Verification`. Each test asserts the intended behaviour, so a failing test means the bug is present. Once a bug is fixed its test turns green and stays as a regression test.

- `EditMode/DomainBugTests.cs` covers death, score and attack timing in the domain services.
- `EditMode/SpawnerBugTests.cs` covers the random attack factories and tile tracking in `PlayerAttacker`.
- `PlayMode/SpawnerCoroutineBugTests.cs` covers wave coroutines and their timing.
- `PlayMode/SceneBugTests.cs` loads `SampleScene` and checks the danger tile warnings.

Tests in category `KnownBug` are expected to fail until fixed. Tests in `DesignQuestion` show behaviour that may or may not be intended.

Run them from the Test Runner window, or from a terminal with the editor closed:

    Unity.exe -batchmode -nographics -projectPath . -runTests -testPlatform EditMode -testResults editmode.xml
    Unity.exe -batchmode -nographics -projectPath . -runTests -testPlatform PlayMode -testResults playmode.xml

The build finding was checked with a Windows player build:

    Unity.exe -batchmode -nographics -quit -projectPath . -buildTarget Win64 -buildWindows64Player build/Game.exe

Keep `-nographics`. Without it the Unity Version Control plugin opens a login dialog nobody can see, and the run hangs forever.

Status values:

- Confirmed: a test or build reproduces it.
- Confirmed by reading: the code path is unambiguous, no test written.
- Latent: the code is wrong but current assets and flow never trigger it.
- Refuted: looked like a bug, is not one.
- Design question: works as written, may not be what you want.

## Summary

| ID | Finding | Severity | Status |
|----|---------|----------|--------|
| B8 | Player builds fail on stray `using Codice...` imports | Blocker | Confirmed |
| B1 | Every hit after death triggers death again | High | Confirmed |
| B2 | Score keeps rising after death and when quitting to menu | High | Confirmed |
| B3 | Wave coroutines cannot be stopped | High | Confirmed |
| B5 | Spawning runs on Unity time, attacks run on game time | Medium | Confirmed |
| B6 | Tile tracking forgets tiles shared by two attacks | Medium | Confirmed |
| B6b | Random attacks can cover the whole arena | Design | Confirmed |
| B7 | Spear hitbox runs ahead of the spear graphic | Medium | Confirmed by reading |
| B7b | Spears can skip a tile at very low frame rates | Low | Confirmed below about 15 fps |
| N1 | Each stage change drops leftover time | Low | Confirmed |
| N2 | One shared pause flag for every pauser | Low | Latent |
| N3 | Spear trail fade compounds every frame | Low | Confirmed by reading |
| N4 | Material copies are never destroyed | Low | Confirmed by reading |
| B4 | Danger warning bookkeeping is unbalanced | Low | Latent |
| B9 | Hammer crashes without an Animator or clip | Low | Latent |
| N5 | Dead code and leftovers | Cleanup | Confirmed by reading |
| R1 | `using NUnit.Framework` in `GameBootstrap` breaks builds | None | Refuted |

## Findings

### B8. Player builds fail on stray `using Codice...` imports

Severity blocker. Confirmed with a Windows player build from the command line, which fails:

    Assets\Scripts\Domain\Models\Attack\Spear\SpearColumnAttack.cs(1,7): error CS0246: The type or namespace name 'Codice' could not be found

`SpearColumnAttack.cs` has `using Codice.Client.Common` and `SpearPresentationBase.cs` has `using Codice.CM.Common`. Codice lives in `unityplastic.dll` from the Unity Version Control package, which is Editor only and auto-referenced, so the editor compiles fine and every player build fails. Neither file uses anything from it. They look like IDE auto-imports. The build stops at the Domain assembly, so the second import is not reported yet but will fail the same way.

Fix: delete both lines.

### B1. Every hit after death triggers death again

Severity high. Confirmed by `B1_PlayerDiesOnlyOnce_WhenHitAgainAfterDeath` (2 deaths, expected 1).

`PlayerService.DamagePlayer` fires `OnPlayerDied` whenever the player is dead after the call, including when the player was already dead and `Player.TakeDamage` ignored the hit. The game stays in `GameState.Gameplay` during the 5 second death slow motion, so attacks keep ticking and hitting the corpse. Each extra hit runs `DeathPresenter.HandlePlayerDeath` again: it restarts the slow motion, camera zoom and fall animation, and queues another Game Over screen.

Where: `Assets/Scripts/Domain/Services/Player/PlayerService.cs`, `DamagePlayer` and `CheckForHits`.

### B2. Score keeps rising after death and when quitting to menu

Severity high. Confirmed by two tests:

- `B2_ScoreFreezes_AfterDeath`: score went from 0 at death to 2.
- `B2_ReturningToMenu_DoesNotAwardOrSaveScoreForCancelledAttacks`: 3 cancelled attacks gave 3 points, and a high score of 3 was saved.

Score is +1 per `OnAttackCompleted`. Attacks keep completing during the death slow motion, and `PlayerAttacker` keeps spawning because the state is still `Gameplay`. On return to the menu, `AttackService.ClearAllAttacks` raises `OnAttackCompleted` for every cancelled attack. `AttackService` subscribes to the state change before the save handler in `GameBootstrap`, so the inflated high score is written to PlayerPrefs. The Game Over screen reads the score 2.5 seconds after death, so it shows a lower number than the one that gets saved.

Where: `PlayerService` constructor, `AttackService.ClearAllAttacks`, `GameBootstrap.InitializePlayerComponents`.

### B3. Wave coroutines cannot be stopped

Severity high. Confirmed by two PlayMode tests:

- `B3_ReturningToMenu_CancelsTheWaveInProgress`: 4 attacks spawned, expected 1, and they sit in the main menu.
- `B3_NewWave_ReplacesThePreviousWave`: 6 attacks spawned, expected 4.

`SpawnAttackWave` runs its work with `yield return StartCoroutine(...)`. `StopCoroutine` on the outer coroutine leaves the inner one running. The `StopCoroutine` calls in `OnTimeTick` and `StopSpawning` therefore do nothing useful. Old waves keep spawning next to new ones, and a wave that was running when you quit spawns attacks into the main menu. Those wait frozen (time is paused) and go live when the next run starts.

Where: `Assets/Scripts/Adapter.Gameplay/PlayerAttacker.cs`, `SpawnAttackWave`, `OnTimeTick`, `StopSpawning`.

### B5. Spawning runs on Unity time, attacks run on game time

Severity medium. Confirmed by `B5_WaveWaits_WhileDomainTimeIsPaused` (3 attacks spawned while time was paused, expected 1).

Attack stages advance on domain ticks, which `DomainUpdater` scales and `TimeService` can pause. Random waves wait with `WaitForSeconds` and sequences compare against `Time.time`, both Unity time. Every hit pauses domain time for 0.2 seconds (`PlayerHitPresenter`) and death slows it to zero, yet spawning continues at full speed. Sequence step delays also drift away from the attacks they are meant to line up with.

Where: `PlayerAttacker.SpawnMultipleAttacksWithDelay` and `SpawnAttackSequence`.

### B6. Tile tracking forgets tiles shared by two attacks

Severity medium. Confirmed by `B6_TileStaysTracked_WhileAnotherAttackStillCoversIt`.

`PlayerAttacker._attackedTiles` is a `HashSet`. The hammer factory allows overlap with tiles already under attack (it only needs 50% of its own tiles free). When one of two overlapping attacks finishes, the shared tile is removed while the other attack still covers it. Later spawns then treat the tile as free.

Where: `PlayerAttacker.OnAttackAdded` and `OnAttackCompleted`.

### B6b. Random attacks can cover the whole arena

Design question. Confirmed by `B6_RandomSpears_LeaveSomeOfTheArenaFree` (4 row spears covered all 12 tiles).

`_minFreePercentage` is checked against the tiles of the new attack only, never against the arena. Spear factories go further and require their whole row or column to be free, so for spears the percentage check never matters. With up to 4 simultaneous attacks on a 3x4 grid, four row spears cover every tile. Spears sweep one tile at a time, so this is not always unsurvivable, but nothing guarantees a safe tile.

Where: the three factories in `Assets/Scripts/Adapter.Gameplay/Attack Factory`.

### B7. Spear hitbox runs ahead of the spear graphic

Severity medium. Confirmed by reading, with the numbers worked out from the code.

The dangerous tile is `floor(progress * tileCount)` along the row or column. The graphic travels from 2.3 tiles before the first tile to 2 tiles past the last one. On a column of 4, tile 1 becomes deadly while the spear is still about 1.5 tiles short of it. Players get hit by a spear that has visibly not reached them yet.

Where: `LinearProgressivePattern.GetActiveDangerPositions` and `SpearPresentationBase.CalculateSpearTrajectory`.

### B7b. Spears can skip a tile at very low frame rates

Severity low. Confirmed by `B7_SpearColumn_AtMaxDifficulty_EndangersEveryTile`. It passes at 60 and 30 fps and fails at 12 fps, where tile 2 was never dangerous.

Hits are only checked for the tile under the spear on each tick. At maximum difficulty a column spear crosses a tile in 0.05 seconds, so a single frame longer than that (a hitch, a GC spike) can jump over a tile entirely.

### N1. Each stage change drops leftover time

Severity low. Confirmed by `N1_AttackLasts_ItsConfiguredDuration`. A spear configured for 1.40 seconds lasted 1.45 seconds at 60 fps and 1.47 seconds at 30 fps.

`Attack.AdvanceToNextStage` resets `ElapsedTimeInStage` to 0 and throws away the overshoot, and at most one stage advances per tick. Zero-length stages (spear Impact and Recovery) still cost a full tick each. Attacks therefore run longer at lower frame rates.

Where: `Assets/Scripts/Domain/Models/Attack/Attack.cs`, `OnTick` and `AdvanceToNextStage`.

### N2. One shared pause flag for every pauser

Severity low. Latent.

`TimeService` has a single `IsPaused` bool. The game state change and `PlayerHitPresenter` both set it, and the hit presenter's `Resume` after 0.2 seconds would undo a pause someone else started. No current flow overlaps the two, but a pause menu or a longer hit stop would.

Where: `TimeService`, `PlayerHitPresenter.PauseCoroutine`.

### N3. Spear trail fade compounds every frame

Severity low, cosmetic. Confirmed by reading.

`SpearPresentationBase.MovementFadeOut` reads the trail gradient it already changed last frame and shifts it again, so the fade speeds up the longer it runs instead of following `fadeProgress`. The original gradient should be cached once at spawn.

### N4. Material copies are never destroyed

Severity low. Confirmed by reading.

Accessing `Renderer.material` creates a copy of the material per renderer. The spear and hammer presentations do this for every attack, and `Destroy` only destroys the GameObject. The copies stay in memory until a scene unload, and this game never unloads its scene.

### B4. Danger warning bookkeeping is unbalanced

Severity low. Latent. Confirmed by `B4_QuittingMidWarning_ReleasesTheWarning`, which checks the count before the arena is rebuilt.

My first read was that quitting during a warning leaves tiles stuck red. That does not happen: `ArenaPresenter` destroys the tiles on the menu and `TileVisualizer.OnArenaVisualized` resets all counts when the arena is rebuilt. The bookkeeping is still wrong in two ways, and both will bite once attacks can be cancelled mid-run:

- `Destroy()` on the presentations never releases the warning it added.
- `DisableCommitWarning` runs on Recovery even if `EnableCommitWarning` never ran. That happens when a long frame skips the window where the warning turns on. The extra decrement can then hide another attack's warning early.

Where: `AttackPresentationBase`, `HammerPresentation`, `SpearPresentationBase`.

### B9. Hammer crashes without an Animator or clip

Severity low. Latent.

`HammerPresentation.UpdateTelegraphStage` calls `_animator.SetTrigger` without a null check, and `ChangeAnimatorSpeed` uses the clip it looked up by name without checking that it exists. The current hammer prefab has an Animator, and all four clip names match `Hammer Animator.controller`, so nothing fails today. Renaming a clip would throw every frame.

### N5. Dead code and leftovers

- `TimeService.TotalElapsedTime` is never updated and is always 0.
- `AttackPresenter._commitAudioEvent` is serialized but never used.
- `AttackPresenter.RegisterVisualBehavior` and `AttackFactoryRegistry.CreateSpecificAttack` have no callers.
- `DebugAttackPresenter` has `[SerializeField] private IArena _arena`. Unity cannot serialize interfaces, so the field never shows in the inspector.
- `_initializeDebugComponents` is on in `SampleScene`. The gizmos only draw in the editor, but its toggle input action is enabled in builds.

### R1. `using NUnit.Framework` in `GameBootstrap` breaks builds

Refuted. `nunit.framework.dll` from `com.unity.ext.nunit` is enabled for all platforms and auto-referenced, so the import compiles in a player build. It is unused and can be deleted.

## Adding a new attack

Adding one touches about 10 places across three assemblies:

1. A domain `Attack` subclass, plus a pattern if the existing ones do not fit.
2. An `IAttackFactory` implementation.
3. A registration line in `PlayerAttacker.InitializeAttackFactories`.
4. The `AttackType` enum and both mapping switches in `AttackFactoryRegistry`.
5. A parameters class in `AttackStepParameters.cs`.
6. A case in the `AttackSequenceDefinition.Validate` switch.
7. An `AttackDefinitionBase` ScriptableObject subclass.
8. A presentation class, its factory, and visual and audio config assets.
9. A case in the `AttackPresentationFactoryBuilder` switch.
10. The definition asset added to the lists on both `PlayerAttacker` and `AttackPresenter` in the scene.

Step 10 fails quietly. An attack missing from the `AttackPresenter` list is invisible but still deals damage, and the only sign is a log warning.

Ways to shrink this:

- Let each definition ScriptableObject create its own gameplay factory and presentation factory. That removes the three switches in steps 4, 6 and 9.
- Keep one list of definitions and share it between spawning and presentation.
- Drop the `AttackType` enum and identify sequence steps by their parameters type, which `[SerializeReference]` already stores.
- Move `MeetsFreePercentageRequirement`, copied into all three factories, into one shared helper.

## Architecture

The core holds up. `Attack` is a clean stage machine, `IAttackPattern` keeps "where is it dangerous" separate from "what stage is it in", and presentation only listens to domain events. Keep all of that.

The problems sit in the adapter layer. Gameplay rules live in a MonoBehaviour, the same idea is described three different ways, and visuals decide things that affect fairness.

### A1. Split `PlayerAttacker` and move its rules into the domain

`PlayerAttacker` holds the difficulty curve, wave cadence, sequence playback, placement rules and occupancy tracking, all running on coroutines. Those are gameplay rules. Split them into a pure difficulty curve, a scheduler for waves and sequences ticked by `ITimeService`, and one placement rules component. `PlayerAttacker` is left as a thin adapter that passes inspector config in.

Fixes B3, B5 and B6. The spawner becomes testable in EditMode.

### A2. One way to describe "spawn this attack here"

Today there are three: random spawns go through `AttackDefinitionBase` plus factory code that picks a placement, sequences go through the `AttackType` enum plus `AttackStepParameters`, and the domain has its own constructors.

Proposal: each factory lists every valid placement as parameters objects. A shared selector filters those candidates by the fairness rules and picks one. Sequences already produce parameters objects, so both paths end in the same build step. The fairness rules then live in one place (B6b), and the copied `MeetsFreePercentageRequirement` disappears.

Belongs with A1.

### A3. Warning timing is a gameplay rule

When tiles turn red is hardcoded in the presentations: 75% of telegraph for spears, 90% for hammers. How much warning the player gets decides fairness, so it should be data in `AttackTiming` or its own domain stage. Then it can be tuned, tested and guaranteed at high difficulty.

Depends on topic 3.

### A4. One source of truth for spear position

The pattern returns a tile index and the presentation computes its own path, which is how B7 happens. Let `IProgressivePattern` expose one continuous position along its axis, and have both hit detection and the graphic read it.

Depends on topic 4.

### A5. Stop looking things up by C# type

The factory registry, `AttackPresenter` and `AttackPresentationFactoryBuilder` each map a type to an implementation. If each definition asset creates its own gameplay factory and presentation factory from one shared list, all three lookups go away. This is the main step in shrinking the "Adding a new attack" list above.

Can wait until a fourth attack is planned.

### C1. Cuts

These can be removed at any time:

- The `AttackType` enum, both mapping switches in `AttackFactoryRegistry`, and the unused registry overload `CreateSpecificAttack(parameters, timing)`. The `[SerializeReference]` parameter type already identifies the attack.
- `IAttackPresentationFactoryBuilder` and its implementation. One implementation exists, and it disappears with A5.
- `SpearPresentationBaseFactory` and its two subclasses, three classes that each call one constructor.
- `IAttackDefinition`, which wraps one property of the base class.
- Dead members: `AttackPresenter.RegisterVisualBehavior`, `AttackSequenceDefinition.GetTotalDuration`, `difficultyRating` unless topic 6 uses it, and `_telegraphIndicators` in `SpearPresentationBase`, which is created and cleared but never filled.

`SpearRowAttack` and `SpearColumnAttack` are mostly type tags, but the presentations read their `Direction`. Merging them only pays off after A5.

### Suggested order

The cuts can go first. A1 and A2 go together, after the death state decision in topic 1. A3 and A4 follow the fairness and hitbox discussions. A5 waits until a new attack is planned.

## Topics to discuss

1. What happens when the player dies. There is no game over state, only `MainMenu` and `Gameplay`, so death is purely visual and the simulation keeps running. A `GameOver` state that stops hits, scoring and spawning would fix B1, B2 and most of B5 at the root, instead of patching each one.

2. Where spawning lives. Waves and sequences are Unity coroutines on a MonoBehaviour, while attacks live in the domain. Moving wave and sequence scheduling into a domain service driven by `OnTick` would fix B3 and B5 together, make the spawner testable without PlayMode, and give one clock for everything.

3. Fairness guarantees. Decide what the game promises the player, for example a minimum number of free tiles, a safe tile always one step away, or a minimum warning time. Today the tile warning is 25% of the spear telegraph and 10% of the hammer telegraph, about 0.125 seconds for both at maximum difficulty. Then enforce it in one place (B6, B6b).

4. Hitbox or graphic, which one leads. Either the danger tile follows the spear's real position, or the spear's path is fitted to the danger timing (B7). Related: should a fast attack check every tile it crossed since the last tick (B7b)?

5. Scoring. Score is +1 per completed attack, so a six-step sequence is worth six points and cancelled attacks count too. Survival time, waves cleared or near misses are alternatives worth weighing before fixing B2 in place.

6. Difficulty curve. The ramp is linear over 300 seconds on spawn interval and timing. Sequences ignore difficulty apart from timing, `difficultyRating` is unused, and at the 0.7 second minimum interval a new wave starts before longer sequences finish. Difficulty tiers that choose which sequences can appear are one option.

7. The extensibility refactor described in the previous section. It is worth doing before adding a fourth attack.

8. Frame-rate independence (N1, B7b). Carrying leftover time into the next stage is a small change, but it shifts every attack's timing slightly, so it should be a conscious choice.
