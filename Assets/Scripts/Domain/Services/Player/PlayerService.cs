using Domain.Models.Player;
using Domain.Ports;
using System;
using UnityEngine;

namespace Domain.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly Player _player;
        private readonly IArena _arena;
        private readonly IAttackService _attackService;
        private readonly ILogger _logger;
        private readonly ITimeService _timeService;

        private int _highScore;
        private int _score;
        private int _gold;

        public event Action OnPlayerAttackHit;
        public event Action<GridPosition, GridPosition> OnPlayerMove;
        public event Action OnPlayerDied;
        public event Action<int> OnPlayerScoreChange;
        public event Action<int> OnPlayerHighScoreChange;
        public event Action<int> OnPlayerGoldChange;

        public PlayerService(Player player, IArena arena, IAttackService attackService, ITimeService timeService, ILogger logger = null)
        {
            _player = player;
            _arena = arena;
            _attackService = attackService;
            _timeService = timeService;
            _logger = logger;

            _timeService.OnTick += OnTick;

            _attackService.OnAttackCompleted += attack => GivePlayerScore(1);

            Score = 0;
            Gold = 0;
        }

        private void OnTick(float deltaTime)
        {
            CheckForHits();
        }

        public void DamagePlayer(int damage)
        {
            _player.TakeDamage(damage);
            if (_player.State == PlayerState.Dead) OnPlayerDied?.Invoke();
        }

        public int GetPlayerHealth()
        {
            return _player.Health;
        }

        public bool IsPlayerAlive()
        {
            return _player.State == PlayerState.Alive;
        }

        public GridPosition GetPlayerGridPosition()
        {
            return _player.GridPosition;
        }
        
        public Vector3 GetPlayerWorldPosition()
        {
            return _arena.GridToWorld(_player.GridPosition);
        }
        
        public void MovePlayer(Vector2Int direction)
        {
            GridPosition newPosition = new GridPosition(
                _player.GridPosition.GridX + direction.x,
                _player.GridPosition.GridY + direction.y
            );
            MovePlayer(newPosition);
        }

        public void MovePlayer(GridPosition newPosition)
        {
            TileType tileType = _arena.GetTileType(newPosition);
            if (_arena.IsInBounds(newPosition) && tileType == TileType.Floor)
            {
                GridPosition oldPosition = _player.GridPosition;
                _player.Move(newPosition);
                CheckForHits();
                OnPlayerMove?.Invoke(oldPosition, newPosition);
            }
        }

        public void CheckForHits()
        {
            if (_attackService.CheckPlayerHit(_player.GridPosition))
            {
                DamagePlayer(1);
                OnPlayerAttackHit?.Invoke();
            }
        }

        public void GivePlayerScore(int amount)
        {
            if (amount < 0)
                return;

            Score += amount;

            if (Score > HighScore)
            {
                HighScore = Score;
            }
        }

        public void GivePlayerGold(int amount)
        {
            if (amount < 0)
                return;

            Gold += amount;
        }

        public void ResetPlayer()
        {
            GridPosition oldPosition = _player.GridPosition;
            _player.Reset();
            OnPlayerMove?.Invoke(oldPosition, _player.GridPosition);

            Score = 0;
        }

        #region "Properties"
        public int HighScore 
        {
            get { return _highScore; }
            set 
            { 
                _highScore = value;
                OnPlayerHighScoreChange?.Invoke(_highScore);
            } 
        }
        public int Score
        {
            get { return _score; }
            set
            {
                _score = value;
                OnPlayerScoreChange?.Invoke(_score);
            }
        }
        public int Gold
        {
            get { return _gold; }
            set
            {
                _gold = value;
                OnPlayerGoldChange?.Invoke(_gold);
            }
        }
        #endregion
    }
}