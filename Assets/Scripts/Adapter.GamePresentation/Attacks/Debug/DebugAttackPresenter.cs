using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugAttackPresenter : MonoBehaviour
{
    [Header("Debug Settings")]
    [SerializeField] private bool _showDebugVisuals = true;
    [SerializeField] private InputActionReference _toggleAction;

    [Header("Visual Settings")]
    [SerializeField] private Color _activeDangerColor = new Color(1f, 0f, 0f, 0.5f);
    [SerializeField] private float _indicatorHeight = 0.2f;
    [SerializeField] private Vector3 _indicatorSize = new Vector3(0.8f, 0.2f, 0.8f);

    [Header("References")]
    [SerializeField] private IArena _arena;

    private AttackService _attackService;

    public void Initialize(AttackService attackService, IArena arena)
    {
        _attackService = attackService;
        _arena = arena;

        if (_toggleAction != null)
        {
            _toggleAction.action.performed += OnTogglePerformed;
            _toggleAction.action.Enable();
        }
    }

    private void OnDestroy()
    {
        if (_toggleAction != null)
        {
            _toggleAction.action.performed -= OnTogglePerformed;
        }
    }

    private void OnTogglePerformed(InputAction.CallbackContext context)
    {
        _showDebugVisuals = !_showDebugVisuals;
        Debug.Log($"Active Attack Debug Visualizer: {(_showDebugVisuals ? "ENABLED" : "DISABLED")}");
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!_showDebugVisuals || _attackService == null || _arena == null) return;

        Gizmos.color = _activeDangerColor;

        foreach (Attack attack in _attackService.GetActiveAttacks())
        {
            if (attack.CurrentStage == AttackStage.Commit || attack.CurrentStage == AttackStage.Impact)
            {
                AttackState state = attack.GetCurrentState();
                IEnumerable<GridPosition> activeDangerPositions = attack.Pattern.GetActiveDangerPositions(state);

                foreach (GridPosition pos in activeDangerPositions)
                {
                    Vector3 worldPos = _arena.GridToWorld(pos);
                    worldPos.y = _indicatorHeight;
                    Gizmos.DrawCube(worldPos, _indicatorSize);
                }
            }
        }
    }
#endif
}