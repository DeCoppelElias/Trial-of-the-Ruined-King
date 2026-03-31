using System;
using UnityEngine;

/// <summary>
/// Controls the visual representation of a single attack instance.
/// Handles updates, stage changes, and cleanup.
/// </summary>
public interface IAttackPresentation
{
    /// <summary>
    /// Updates the presentation based on attack progress.
    /// Called every frame while attack is active.
    /// </summary>
    void UpdatePresentation(Attack attack);
    
    /// <summary>
    /// Notifies visual that attack stage has changed.
    /// </summary>
    void OnStageChanged(AttackStage newStage);
    
    /// <summary>
    /// Cleans up all visual elements.
    /// </summary>
    void Destroy();

    public event Action<GameObject> OnAttackSpawned;
}