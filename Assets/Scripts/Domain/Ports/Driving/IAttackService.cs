using Domain.Models;
using System;
using System.Collections.Generic;

namespace Domain.Ports.Driving
{
    public interface IAttackService
    {
        event Action<Attack> OnAttackAdded;
        event Action<Attack> OnAttackCompleted;
        void SpawnAttack(Attack attack);
        IEnumerable<Attack> GetActiveAttacks();
        bool CheckPlayerHit(GridPosition playerPosition);
    }
}
