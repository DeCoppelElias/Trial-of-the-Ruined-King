# Game clock

The domain owns time in a run. `ITimeService` is the game clock: it holds the
speed of game time and whether the run is paused, and it advances only on the
real time it is handed. The domain reads no Unity time, which the
[ports and adapters](ports-and-adapters.md) decision already enforces.

Every duration and timer states whether it counts game time or real time.
Game time is what the run experiences, slowed in slow motion and stopped in a
pause. Real time never slows. Anything inside a run counts game time by
default. UI, and anything that must keep moving through a pause, counts real
time. Most timing bugs come from a timer on the wrong clock, so the choice is
made on purpose every time.

One adapter feeds the clock real time each frame and copies the clock's speed
into `Time.timeScale`. Animators, particles, `WaitForSeconds` and
`Time.deltaTime` then follow slow motion and pauses with no code of their
own, and nothing multiplies by the speed by hand. Gameplay facts come only
from the domain. Visuals may use domain ticks or scaled Unity time. Unscaled
Unity time is for real time only. Adapters that want a different speed ask
the clock, never `Time.timeScale` or each other.

The main menu is not a pause. No run exists there, no game time passes and
the speed is 1, so menu visuals need no unscaled settings. A pause means a run
frozen mid-play, such as hit stop or a pause menu.

Every pause and every speed change is a request to the clock, which hands
back a hold. The request lasts until its owner releases the hold, or, when
the request was given a length, until the clock has counted that length. The
run is paused while any pause is held. Otherwise the speed is the slowest one
any hold asks for, and 1 when none is held. An owner can release only its own
hold, so a short hit stop ending never unpauses a pause menu, and a short
slow motion ending never cancels a death. A request with a known length, such
as hit stop, gives it to the clock rather than timing it in an adapter, so it
lasts the same number of steps on every device. A ramp from one speed to
another counts real time, since a ramp toward 0 counted in game time would
never arrive, and it freezes while the run is paused, so unpausing resumes it
where it stopped. When a run ends the clock drops every hold and the speed
returns to 1, so nothing in the next run waits on a speed left at 0.
Releasing a dropped hold does nothing.

The clock steps 60 times per second of real time, set in one place, and
carries what is left to the next frame. Each step advances game time by the
speed times 1/60 of a second, and by nothing during a pause. A frame can fire
several steps or none, and at most a small fixed number, so a long hitch or a
return from the background slows the game briefly rather than running a
burst of steps at once. Fixed steps make hits and timing the same at any
frame rate, which matters on mobile, and make a run repeatable from its
inputs, which seeded runs need. Steps per real second, rather than per game
second, keep slow motion as smooth as normal play with no interpolation. At
normal speed the domain updates 60 times a second, so on a faster screen
visuals drawn from domain state move every other frame, which is accepted.
