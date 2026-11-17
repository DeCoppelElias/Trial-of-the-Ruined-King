# AGENTS.md

A survival dodging game where the player dodges endless waves of escalating attacks on a grid arena, chasing high scores and gold.

## Documents

- `DESIGN.md` what Trial of the Ruined King is for. Read it before proposing a feature.
- `CONTEXT.md` the glossary. Use its words.
- `ARCHITECTURE.md` the map of the code as it is. Read it before changing code.
- `docs/adr/` the decisions, one file per topic. Its README maps them by area.
  Read the ones that bear on a change before making it.
- `docs/ideas/` what is wanted and not decided. A new idea goes here without
  asking, in the format its README gives.
- `docs/roadmap/` the committed build order, listed in its README.
- `.plans/` implementation plans for an entry, one file each. It is gitignored,
  so a plan is working material and never a document.

`README.md` is for players and visitors to the repo. Agent information never
goes there. Each document says at its top what it holds and never holds. When you are not
sure where something belongs, ask EliasDeCoppel rather than picking. Link a
decision or an entry rather than restating it.

`groundwork-docs-check` checks links, indexes and status lines. Run it after
changing any document.

## Asking

Ask EliasDeCoppel before promoting an idea onto the roadmap, changing an accepted
decision, adding or reordering roadmap entries, and before any push, merge,
pull request or tag. Asking means giving the options with a recommendation and
waiting, not deciding and asking for confirmation afterwards.

When a document has drifted or holds something outside its lane, report it
with the fix you propose, and change it only after EliasDeCoppel says go.

A grill writes nothing to the documents until it ends. Then it sends a summary
of every choice and the changes it makes, and applies them on a branch after
EliasDeCoppel says go.

## Roadmap entries

An entry is built on its own branch, `entry-<slug>`. Grill the details it
leaves open before planning. It is finished when the tests pass, the code is
reviewed, its status reads `done`, `ARCHITECTURE.md` describes any code it
moved or added, any idea it built is marked `built`, any choice made on the
way is in its decision, and `groundwork-docs-check` is clean. Then it merges
on EliasDeCoppel's go.

## Build and run

Unity 6000.3.9f1. Open the project in the Unity Editor to run it, or run the
tests from the Test Runner window. From a terminal, close the editor first.
`<Unity>` below is the path to that editor's `Unity.exe`.

- EditMode tests: `<Unity> -batchmode -nographics -projectPath . -runTests -testPlatform EditMode -testResults editmode.xml`
- PlayMode tests: `<Unity> -batchmode -nographics -projectPath . -runTests -testPlatform PlayMode -testResults playmode.xml`
- Windows build: `<Unity> -batchmode -quit -projectPath . -buildTarget Win64 -buildWindows64Player build/Game.exe`

Wait on the Unity process itself, not on its output. The AI Assistant package
starts `relay_win.exe`, which outlives Unity and keeps its output pipe open, so
a shell that pipes or captures Unity's output never returns. In PowerShell,
start Unity with `Start-Process -PassThru`, call `WaitForExit()` on the result,
then stop any `relay_win` process left behind.

`tools/run-tests.ps1` does all of this. It finds the editor through Unity
Hub, runs one suite or both, prints the totals and every failure, and lists
compile errors when no results come out. It exits 1 unless every suite passes.

"The tests pass" means both suites pass in full. The
[testing](docs/adr/testing.md) decision says what an entry must test.
