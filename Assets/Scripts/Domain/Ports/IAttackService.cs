using System;
using System.Collections.Generic;

public interface IAttackService
{
    event Action<Attack> OnAttackAdded;
    event Action<Attack> OnAttackCompleted;
    void SpawnAttack(Attack attack);
    IEnumerable<Attack> GetActiveAttacks();
    bool CheckPlayerHit(GridPosition playerPosition);
}