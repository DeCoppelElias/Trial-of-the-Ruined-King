using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Attacks/Spear/SpearColumnAttackDefinition")]
public class SpearColumnAttackDefinition : AttackDefinitionBase
{
    public SpearVisualConfig visualConfig;
    public SpearAudioConfig audioConfig;

    public override Type AttackType => typeof(SpearColumnAttack);
}