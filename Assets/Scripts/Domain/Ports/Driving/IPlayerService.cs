using Domain.Models;
using System;

namespace Domain.Ports.Driving
{
    public interface IPlayerService
    {
        int HighScore { get; set; }
        int Score { get; set; }
        int Gold { get; set; }

        event Action OnPlayerAttackHit;
        event Action<int> OnPlayerScoreChange;
        event Action<int> OnPlayerHighScoreChange;
        event Action<int> OnPlayerGoldChange;
        event Action<GridPosition, GridPosition> OnPlayerMove;
        event Action OnPlayerDied;
        void DamagePlayer(int damage);
        void GivePlayerScore(int amount);
        void GivePlayerGold(int amount);
        int GetPlayerHealth();
        bool IsPlayerAlive();
        GridPosition GetPlayerGridPosition();
        void MovePlayer(GridDirection direction);
    }
}