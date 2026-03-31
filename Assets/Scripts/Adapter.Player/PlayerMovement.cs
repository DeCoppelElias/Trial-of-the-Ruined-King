using Domain.Ports;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public enum MovementState
{
    Idle,
    WindingUp,
    Dodging,
    Recovering
}

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    private float _recoveryDelay = 0.1f;

    private IPlayerService _playerService;
    private MovementState _state = MovementState.Idle;
    private Vector2Int _pendingDirection;
    private bool _hasPendingInput;
    
    public void Initialize(IPlayerService service)
    {
        _playerService = service;
    }
    
    public void OnMove(InputValue value)
    {
        // Don't process input if player is dead
        if (!_playerService.IsPlayerAlive())
            return;

        Vector2 input = value.Get<Vector2>();
        Vector2Int direction = Vector2Int.zero;
        
        // Convert analog input to discrete grid directions
        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
        {
            direction = input.x > 0 ? Vector2Int.right : Vector2Int.left;
        }
        else if (input.y != 0)
        {
            direction = input.y > 0 ? Vector2Int.up : Vector2Int.down;
        }
        
        if (direction != Vector2Int.zero)
        {
            // Store input if currently committed
            if (_state != MovementState.Idle)
            {
                _pendingDirection = direction;
                _hasPendingInput = true;
            }
            else
            {
                StartCoroutine(ExecuteMovement(direction));
            }
        }
    }
    
    private IEnumerator ExecuteMovement(Vector2Int direction)
    {
        _hasPendingInput = false;
        _playerService.MovePlayer(direction);
        
        // Recovery phase: brief cooldown
        if (_recoveryDelay > 0f)
        {
            _state = MovementState.Recovering;
            yield return new WaitForSeconds(_recoveryDelay);
        }
        
        _state = MovementState.Idle;
        // Process pending input if any
        if (_hasPendingInput)
        {
            StartCoroutine(ExecuteMovement(_pendingDirection));
        }
    }
}