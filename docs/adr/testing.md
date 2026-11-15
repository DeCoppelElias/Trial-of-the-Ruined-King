# Testing

Tests go where the rules are. Every domain rule an entry adds or changes gets
an EditMode test. An adapter gets an EditMode test only for logic of its own
that cannot live in the domain, such as the tile to world mapping in
`ArenaSpace`. One PlayMode smoke test loads the scene, starts a run from the
main menu and fails on any logged error or if no attack spawns. Presentation,
meaning visuals, audio and UI layout, is not tested. Whether a test is written
before or after the code is left open. This decision sets only what exists
when an entry is done.

The [ports and adapters](ports-and-adapters.md) decision keeps the rules in a
Unity-free domain, so they run as plain EditMode tests without a scene. PlayMode
tests are slow and test mostly thin wiring, so they are kept to what nothing
else can reach. A new PlayMode test is justified in its entry.

A bug fix gets a test when the behaviour it restores is worth guarding. A typo
fix gets none. The test lives with the other tests for that code and is named
for the behaviour, not the bug. There are no tests that are expected to fail,
and no categories to filter them out, so "the tests pass" means both suites
pass in full.

The test assemblies mirror the production ones. `Domain.Tests` references only
the domain. An adapter gets `<Adapter>.Tests` once it has a test.
`PlayMode.Tests` holds the smoke test and is the one test assembly that loads
the scene.

An EditMode test never waits on real time and never touches disk or
PlayerPrefs. It uses fakes for the driven ports, kept in `Domain.Tests`. The
exception is a test that checks content assets through the editor's
`AssetDatabase`, such as the one that validates every attack sequence. The
suite has no time target, since these rules are what keep it fast.
