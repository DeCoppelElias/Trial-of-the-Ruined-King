


using System;

namespace Domain.Models.Player
{
    public class Player
    {
        private ILogger _logger;

        private int _health;
        public PlayerState State { get; private set; }
        public GridPosition GridPosition { get; private set; }

        private readonly int _initialHealth;
        private readonly GridPosition _initialPosition;

        public int Health
        {
            get => _health;
            private set {
                _health = value < 0 ? 0 : value;
            }
        }

        public Player(int initialHealth, GridPosition initialPosition, ILogger logger = null)
        {
            _logger = logger;
            if (initialHealth <= 0)
                initialHealth = 1;
            
            _initialHealth = initialHealth;
            _initialPosition = initialPosition;

            _health = initialHealth;
            State = PlayerState.Alive;
            GridPosition = initialPosition;
        }

        public void TakeDamage(int damage)
        {
            if (State == PlayerState.Dead)
                return;

            if (damage < 0)
                damage = 0;

            Health -= damage;
            _logger?.LogInfo($"Player took {damage} damage, remaining health: {Health}");

            if (Health <= 0) Die();
        }

        public void Move(GridPosition newPosition)
        {
            GridPosition = newPosition;
        }

        private void Die()
        {
            State = PlayerState.Dead;
            _logger?.LogInfo("Player has died.");
        }

        public void Reset()
        {
            Health = _initialHealth;
            State = PlayerState.Alive;
            GridPosition = _initialPosition;
            _logger?.LogInfo("Player has been reset.");
        }
    }
}

