using UnityEngine;

/// <summary>
/// Defines how a specific attack type should be visualized.
/// Allows different attacks to have completely different visual behaviors.
/// </summary>
public interface IAttackPresentationFactory
{
    /// <summary>
    /// Creates the visual and audio representation for this attack.
    /// </summary>
    /// <param name="attack">The attack to present</param>
    /// <returns>Visual and audio representation for this attack</returns>
    IAttackPresentation CreatePresentation(Attack attack);
}