using System;
using System.Collections.Generic;
using Domain.Ports;

public class AttackService : IAttackService
{
    private readonly List<Attack> _activeAttacks;
    
    public event Action<Attack> OnAttackAdded;
    public event Action<Attack> OnAttackCompleted;

    public AttackService(ITimeService timeService, IGameStateService gameStateService)
    {
        _activeAttacks = new List<Attack>();
        timeService.OnTick += OnTick;

        gameStateService.OnStateChanged += OnGameStateChanged;
    }

    private void OnTick(float deltaTime)
    {
        for (int i = _activeAttacks.Count - 1; i >= 0; i--)
        {
            Attack attack = _activeAttacks[i];
            attack.OnTick(deltaTime);
            
            if (attack.IsComplete())
            {
                _activeAttacks.RemoveAt(i);
                OnAttackCompleted?.Invoke(attack);
            }
        }
    }

    public void SpawnAttack(Attack attack)
    {
        _activeAttacks.Add(attack);
        OnAttackAdded?.Invoke(attack);
        attack.Start();
    }

    public IEnumerable<Attack> GetActiveAttacks()
    {
        return _activeAttacks;
    }

    public void ClearAllAttacks()
    {
        for (int i = _activeAttacks.Count - 1; i >= 0; i--)
        {
            Attack attack = _activeAttacks[i];
            _activeAttacks.RemoveAt(i);
            OnAttackCompleted?.Invoke(attack);
        }
    }

    /// <summary>
    /// Checks if player position is currently being attacked.
    /// Returns true if damage was dealt (once per attack).
    /// </summary>
    public bool CheckPlayerHit(GridPosition playerPosition)
    {
        foreach (var attack in _activeAttacks)
        {
            // Skip if already dealt damage
            if (attack.HasDealtDamage)
                continue;

            // Let each attack determine its own hit logic
            if (attack.IsPositionUnderAttack(playerPosition))
            {
                attack.MarkDamageDealt();
                return true;
            }
        }
        
        return false;
    }

    private void OnGameStateChanged(GameState previousState, GameState newState)
    {
        if (newState == GameState.MainMenu)
        {
            ClearAllAttacks();
        }
    }
}
