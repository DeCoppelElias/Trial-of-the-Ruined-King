using System;
using UnityEngine;

public abstract class AttackDefinitionBase : ScriptableObject, IAttackDefinition
{
    public AttackTiming attackTiming;
    public abstract Type AttackType { get; }
}