using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Attacks/Hammer/HammerAttackDefinition")]
public class HammerAttackDefinition : AttackDefinitionBase
{
    public HammerVisualConfig visualConfig;
    public HammerAudioConfig audioConfig;
    public override Type AttackType => typeof(HammerAttack);
}
