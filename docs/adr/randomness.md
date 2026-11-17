# Randomness

Every roll in the game comes from a `RandomSource`, a plain class in
`Domain.Models` that wraps one `System.Random` and adds the helpers the game
needs: int and float ranges, a value from 0 to 1, a coin flip, a pick from a
list, and `Reseed`. The game uses only `System.Random` and never calls
`UnityEngine.Random`. This is a rule for review, with no test to enforce it.
The domain cannot reference Unity, and
`UnityEngine.Random` is one global state that any package can draw from, so
one library for both sides keeps every roll owned by whoever holds it.

`RandomSource` is not a port. It is .NET arithmetic with nothing outside to
swap, and tests get repeatable rolls from a fixed seed. If a test ever needs to
script an exact roll, an interface can be extracted then.

`Bootstrap` builds two instances. `runRandom` makes every gameplay roll: the
arena, coins, which attacks spawn, where, and when. `cosmeticRandom` makes
every roll that cannot change what happens in a run, such as shake, audio
pitch and clip, and coin rotation and scale. Adapters may hold either, since
`RandomSource` is a domain model. Each new roll picks its instance on purpose,
because a cosmetic roll on `runRandom` would shift every gameplay roll after
it, so turning shake off would change the attacks.

A run's gameplay rolls begin at its start and come only from its seed. When a
run starts, `runRandom` is reseeded in place, since everything that rolls
holds the same instance. A run given no seed draws one from `Guid.NewGuid()`
and logs it, so an odd run can be replayed in the editor. `cosmeticRandom` is
seeded from a `Guid` too, because on Unity's Mono two unseeded
`System.Random` made in the same tick share a seed.

The same seed gives the same rolls. A full replay of a run from its seed and
inputs also needs spawning on the game clock, which the
[spawning in the domain](../roadmap/domain-spawning.md) entry builds, and
attacks that do not depend on frame rate, which the
[frame rate independence](../roadmap/frame-rate-independence.md) entry builds.
