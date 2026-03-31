# Attack System Documentation

## Overview

The attack system is designed following Domain-Driven Design principles with a clear separation between domain logic and visualization adapters.

## Architecture

### Domain Models (`Assets/Scripts/Domain/Models/Attack/`)

#### `Attack` (Abstract Base Class)
The core domain model that manages attack lifecycle through stages.

**Key Features:**
- State machine managing attack stages (Telegraph ? Commit ? Impact ? Recovery ? Complete)
- Timing management for each stage
- Events for stage transitions
- Position danger detection during Impact stage

**Properties:**
- `CurrentStage`: Current attack stage
- `ElapsedTimeInStage`: Time elapsed in current stage
- `Timing`: Attack timing configuration
- `Pattern`: Attack pattern defining affected tiles

**Events:**
- `OnStageChanged`: Fired when transitioning to a new stage
- `OnAttackComplete`: Fired when attack reaches Complete stage

**Methods:**
- `Update(float deltaTime)`: Advances attack through stages
- `IsDangerousAt(GridPosition)`: Checks if position is dangerous (only during Impact)
- `IsComplete()`: Checks if attack has finished
- `GetStageProgress()`: Returns 0-1 progress through current stage

#### `AttackStage` (Enum)
Defines the four stages of an attack plus completion state.

#### `AttackTiming` (Value Object)
Encapsulates timing configuration for all attack stages.

#### `IAttackPattern` (Interface)
Defines which tiles are affected by an attack.

**Methods:**
- `GetAffectedPositions(IArena)`: Returns all positions hit by attack
- `IsDangerous(GridPosition)`: Checks if specific position is dangerous

#### `RowAttackPattern`
Concrete implementation targeting an entire row.

#### `SpearRowAttack`
First concrete attack implementation - a spear that crosses a row.

**Default Timing:**
- Telegraph: 1.0s (warning indicators)
- Commit: 0.3s (point of no return)
- Impact: 0.2s (damage active)
- Recovery: 0.5s (cooldown)

### Domain Services (`Assets/Scripts/Domain/Services/`)

#### `AttackService`
Manages all active attacks in the game.

**Responsibilities:**
- Update all active attacks
- Spawn new attacks
- Track attack lifecycle
- Check if positions are dangerous

**Events:**
- `OnAttackStarted`: Fired when new attack spawns
- `OnAttackCompleted`: Fired when attack completes

### Adapter Layer (`Assets/Scripts/Adapter.AttackVisualizer/`)

#### `AttackVisualizer`
Unity-specific visualization of attacks using prefabs.

**Configuration:**
- `_telegraphIndicatorPrefab`: Visual for Telegraph stage (warning)
- `_commitIndicatorPrefab`: Visual for Commit stage (locked in)
- `_impactIndicatorPrefab`: Visual for Impact/Recovery stages (danger/cooldown)

**Behavior:**
- Automatically switches visuals when stages change
- Places indicators on affected grid tiles
- Cleans up completed attacks

## Usage Example

### Setup in GameBootstrap or similar:

```csharp
// Create arena
IArena arena = new RectangleArena(10, 10, Vector3.zero);

// Create attack service
AttackService attackService = new AttackService(arena);

// Initialize attack visualizer
AttackVisualizer visualizer = FindObjectOfType<AttackVisualizer>();
visualizer.Initialize(attackService, arena);

// Spawn a spear attack on row 3
SpearRowAttack attack = new SpearRowAttack(3, arena);
attackService.SpawnAttack(attack);

// In Update loop
attackService.Update(Time.deltaTime);
```

### Custom Timing:

```csharp
// Create custom timing for faster attack
AttackTiming fastTiming = new AttackTiming(
    telegraphDuration: 0.5f,
    commitDuration: 0.2f,
    impactDuration: 0.1f,
    recoveryDuration: 0.3f
);

SpearRowAttack fastAttack = new SpearRowAttack(5, arena, fastTiming);
attackService.SpawnAttack(fastAttack);
```

## Design Benefits

### 1. **Separation of Concerns**
- Domain logic is Unity-independent
- Visualization is isolated in adapters
- Easy to test domain rules without Unity

### 2. **Extensibility**
- New attack patterns: Implement `IAttackPattern`
- New attack types: Extend `Attack` base class
- Custom visualizations: Create new visualizer adapters

### 3. **Stage-Based Design**
- Clear communication to players (Telegraph)
- Commitment mechanic (Commit)
- Precise damage window (Impact)
- Breathing room for players (Recovery)

### 4. **Event-Driven**
- Visualizers react to domain events
- Easy to add sound effects, particles, etc.
- Decoupled components

## Future Extensions

### New Attack Patterns
```csharp
public class ColumnAttackPattern : IAttackPattern { ... }
public class AreaAttackPattern : IAttackPattern { ... }
public class CrossAttackPattern : IAttackPattern { ... }
```

### New Attack Types
```csharp
public class HammerSlamAttack : Attack { ... }
public class ArrowVolleyAttack : Attack { ... }
public class SpiralAttack : Attack { ... }
```

### Player Damage Integration
```csharp
// In player health system
if (attackService.IsPositionDangerous(playerPosition))
{
    playerHealth.TakeDamage();
}
```

## Summary

Yes, using an `AttackVisualizer` adapter is an excellent design choice! It:
- Follows your existing architectural patterns (like `ArenaVisualizer`)
- Keeps domain logic pure and testable
- Makes it easy to swap visualizations
- Allows the domain to drive the visuals through events
- Enables independent development of attacks and their visuals
