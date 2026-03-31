using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Attacks/AttackSequenceDefinition", fileName = "NewAttackSequence")]
public class AttackSequenceDefinition : ScriptableObject
{
    [Header("Sequence Identity")]
    [Tooltip("Name of this attack sequence")]
    public string sequenceName;

    [Tooltip("Description of what this sequence does")]
    [TextArea(2, 4)]
    public string description;

    [Header("Sequence Configuration")]
    [Tooltip("List of attacks in this sequence")]
    public List<AttackSequenceStep> steps = new List<AttackSequenceStep>();

    [Header("Base Timing (used if steps don't override)")]
    [Tooltip("Default timing for attacks in this sequence")]
    public AttackTiming baseAttackTiming;

    [Header("Metadata")]
    [Tooltip("Difficulty rating (informational only)")]
    [Range(1, 10)]
    public int difficultyRating = 5;

    public float GetTotalDuration()
    {
        if (steps == null || steps.Count == 0)
            return 0f;

        float maxDuration = 0f;
        foreach (var step in steps)
        {
            var timing = step.overrideTiming ? step.timingOverride : baseAttackTiming;
            float stepEndTime = step.delayFromStart + timing.GetTotalDuration();
            if (stepEndTime > maxDuration)
                maxDuration = stepEndTime;
        }

        return maxDuration;
    }

    public bool Validate(IArena arena, out string errorMessage)
    {
        if (steps == null || steps.Count == 0)
        {
            errorMessage = "Sequence has no steps";
            return false;
        }

        for (int i = 0; i < steps.Count; i++)
        {
            var step = steps[i];
            if (step.parameters == null)
            {
                errorMessage = $"Step {i} has null parameters";
                return false;
            }

            switch (step.attackType)
            {
                case AttackType.SpearRow:
                    if (step.parameters is not SpearRowParameters rowParams)
                    {
                        errorMessage = $"Step {i} (SpearRow) has wrong parameter type: {step.parameters?.GetType().Name}";
                        return false;
                    }
                    if (rowParams.row < 0 || rowParams.row >= arena.Height)
                    {
                        errorMessage = $"Step {i} (SpearRow) row {rowParams.row} out of bounds (0-{arena.Height - 1})";
                        return false;
                    }
                    break;

                case AttackType.SpearColumn:
                    if (step.parameters is not SpearColumnParameters colParams)
                    {
                        errorMessage = $"Step {i} (SpearColumn) has wrong parameter type: {step.parameters?.GetType().Name}";
                        return false;
                    }
                    if (colParams.column < 0 || colParams.column >= arena.Width)
                    {
                        errorMessage = $"Step {i} (SpearColumn) column {colParams.column} out of bounds (0-{arena.Width - 1})";
                        return false;
                    }
                    break;

                case AttackType.Hammer:
                    if (step.parameters is not HammerParameters hammerParams)
                    {
                        errorMessage = $"Step {i} (Hammer) has wrong parameter type: {step.parameters?.GetType().Name}";
                        return false;
                    }
                    if (hammerParams.hammerPosition == null)
                    {
                        errorMessage = $"Step {i} (Hammer) missing position";
                        return false;
                    }
                    if (!arena.IsInBounds(hammerParams.hammerPosition))
                    {
                        errorMessage = $"Step {i} (Hammer) position {hammerParams.hammerPosition} out of bounds";
                        return false;
                    }
                    break;
            }

            if (step.delayFromStart < 0)
            {
                errorMessage = $"Step {i} has negative delay";
                return false;
            }
        }

        errorMessage = null;
        return true;
    }

    private void OnValidate()
    {
        if (steps != null)
        {
            steps = steps.OrderBy(s => s.delayFromStart).ToList();
        }
    }
}
