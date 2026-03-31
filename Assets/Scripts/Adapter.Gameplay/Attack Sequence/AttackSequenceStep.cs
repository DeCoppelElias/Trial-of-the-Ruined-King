using System;
using UnityEngine;

[Serializable]
public class AttackSequenceStep
{
    [Tooltip("Type of attack to spawn")]
    public AttackType attackType;

    [Tooltip("Delay in seconds from the start of the sequence")]
    public float delayFromStart;

    [Tooltip("Parameters specific to the attack type")]
    [SerializeReference]
    public AttackStepParametersBase parameters;

    [Tooltip("Optional: Override the base attack timing for this specific step")]
    public bool overrideTiming;
    public AttackTiming timingOverride;

    public AttackSequenceStep()
    {
        delayFromStart = 0f;
        overrideTiming = false;
    }

    public AttackSequenceStep(AttackType type, float delay, AttackStepParametersBase stepParameters)
    {
        attackType = type;
        delayFromStart = delay;
        parameters = stepParameters;
        overrideTiming = false;
    }
}
