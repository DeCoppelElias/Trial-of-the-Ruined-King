# Demo scope

Status: idea.

## What it does

Defines what the PC demo must prove before the game moves to mobile. This file
holds the owner's original vision document, below, as input for the grill that
turns the demo into roadmap entries.

## Why it is interesting

The demo tests whether the game is fun, and its must-haves become the first
roadmap entries.

## What it would touch

Everything in the demo scope below.

## Open questions

- Which must-haves stay, now that the shop comes after the game is fun?
- In what order they get built.

## Original vision document

Kept as written, apart from ranges like `2–3` becoming `2 to 3`.

### Quick pitch

- One-liner: Dodge increasingly difficult waves of attacks to chase a high score.
- Target vibe (2 to 3 words): Exciting, engaging, learning
- Player promise (why it's fun): The player feels like a real-life ninja, reading patterns, dodging cleanly, and improving run by run while pushing a personal best.

### Current state (today)

- What's playable right now:
    - Gameplay:
        - The player can move around on a grid and dodge attacks.
        - Attacks spawn with different behaviors:
            - Randomly
            - Based on pre-defined patterns
        - The player can earn score and collect gold:
            - Score is earned per dodged attack.
            - Gold spawns randomly on the grid and can be collected.
    - UI:
        - Permanent UI showing high score and gold.
        - Gameplay UI:
            - Current score display.
            - Game over screen.
        - Main Menu UI:
            - Game title.
            - Seamless button to start the game.
    - Graphics and visuals
- Biggest risk / unknown:
    - The player may get bored easily, how to keep the player invested.

### What the game is (in depth)

- Core loop:
    - Dodge increasingly difficult attacks while chasing a high score and collecting gold.
    - Compare high scores with others.
    - Spend gold on upgrades/skins.
    - Get commentary from the Ruined King (feedback, taunts/praise) tied to what happens during the run.

### Demo vs full release

Demo (must prove):

- The dodge feels great: snappy movement, clear hitboxes, clear telegraphs, instant restart.
- A readable difficulty ramp: starts forgiving → quickly becomes intense without feeling unfair.
- Variety before boredom: at least 3 to 5 distinct attack patterns (not just faster random spawns).
- Gold has meaning: 2 to 3 shop choices that noticeably change play.
- The Ruined King hook: reactive voice/text lines (taunts/praise/coaching) tied to performance milestones (streaks, near-misses, death).
- A complete loop: menu → tutorial beat → run → game over → one more run.
- Shareability: local best score + a simple "end card" that tells what's coming in the full game.
- Minimal polish: audio and visual effects that create a mysterious environment.
- Social Links: links to the creator/game socials
- Game analytics?

Full release (expands to):

- More mastery depth: larger pattern library + pattern combinations (mixing types) + advanced telegraphs.
- Challenges: player can try to achieve challenges while in game, this will reward them with additional gold.
- Achievements: achievements to reach certain milestones (score, gold, arena is fully covered with dangerous tiles)
- Shop variety: multiple upgrades.
- Progression that respects time: meta-progression that unlocks options without grinding (new patterns, cosmetics, modifiers).
- More "Kings": extra commentary sets/personalities (or evolving dialogue) so runs stay fresh.
- Multiple arenas / tile sets: new visuals + slight mechanical twists (slippery floor, destructible tiles, hazards).
- More challenge modes: daily seed, score attack rulesets, endurance, "no gold" mode, etc.
- Leaderboards: online leaderboard
- AI agent: Ruined king conversations powered by AI.
- Polish pass: strong audio, VFX juice, accessibility options (colorblind, telegraph clarity, speed scaling).

### Scope & decisions

- Must-have (Demo scope):
    - Dodge feel + fairness: snappy movement, clear hitboxes, and readable telegraphs (avoid "cheap" hits).
    - Difficulty ramp that stays readable: early forgiving → ramps into intensity without becoming visual noise.
    - Attack variety: 3 to 5 distinct attack patterns (patterns you can learn), not just more random spawns.
    - Score run clarity: survival/high-score loop + instant restart + clear "why I died".
    - Ruined King hook: reactive lines tied to milestones (streaks, near-misses, death), enough variety to not repeat constantly.
    - Gold has meaning (demo version): 2 to 3 shop choices that noticeably change play (simple, balance-light).
    - Demo packaging: menu → tutorial beat → run → game over → "what's next" end card.
    - Local best score: at minimum a local best (shareable), online leaderboard later.
    - Minimal polish for atmosphere: A/V feedback that supports the "mysterious ruined king" vibe (not a full juice overhaul).
- Nice-to-have (If time in demo):
    - Cosmetics: if upgrades stay small, let gold buy 1 to 2 skins for motivation.
    - Seeded challenge: a daily/weekly seed (lightweight) for replayability.
    - Basic accessibility toggles: telegraph clarity slider, reduced screenshake.
    - Online leaderboard: leaderboard on which players can compare their scores.
    - AI agent: ruined king dialogue driven by AI.
    - Challenges: player can try to achieve challenges while in game, this will reward them with additional gold.
    - Achievements: achievements to reach certain milestones (score, gold, arena is fully covered with dangerous tiles)
- Won't do (for the demo / this project right now):
    - Campaign content: no levels/biomes/story cutscenes in the demo.
    - Complex meta-progression trees: avoid grinding systems and large unlock trees.
    - Item/loot inventory systems: no roguelite itemization with dozens of items.
    - Multiplayer/PvP: not in scope.
    - Anti-cheat: not needed for demo; only consider after the core is proven.
    - Multiple playable characters: not in scope for demo.

### Notes on the boredom risk

- If players get bored, it's usually one of these:
    - Attacks feel samey → add recognizable patterns + occasional "surprise" behavior.
    - No short-term goals → add milestones (new pattern every X score, mini-peak, perk choice).
    - Feedback is flat → stronger audio/visual feedback + king commentary tied to performance.
    - Too hard too fast → make early game forgiving; ramp later.
