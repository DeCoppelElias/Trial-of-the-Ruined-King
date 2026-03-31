using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Attacks/Spear/SpearRowAttackDefinition")]
public class SpearRowAttackDefinition : AttackDefinitionBase
{
    public SpearVisualConfig visualConfig;
    public SpearAudioConfig audioConfig;

    public override Type AttackType => typeof(SpearRowAttack);
}