[System.Serializable]
public class AttackTiming
{
    public float TelegraphDuration;
    public float CommitDuration;
    public float ImpactDuration;
    public float RecoveryDuration;

    public AttackTiming(float telegraphDuration, float commitDuration, float impactDuration, float recoveryDuration)
    {
        TelegraphDuration = telegraphDuration;
        CommitDuration = commitDuration;
        ImpactDuration = impactDuration;
        RecoveryDuration = recoveryDuration;
    }

    public float GetTotalDuration()
    {
        return TelegraphDuration + CommitDuration + ImpactDuration + RecoveryDuration;
    }

    public float GetDurationForStage(AttackStage stage)
    {
        switch (stage)
        {
            case AttackStage.Telegraph:
                return TelegraphDuration;
            case AttackStage.Commit:
                return CommitDuration;
            case AttackStage.Impact:
                return ImpactDuration;
            case AttackStage.Recovery:
                return RecoveryDuration;
            default:
                return 0f;
        }
    }

    public AttackTiming Multiply(float amount)
    {
        return new AttackTiming(
            TelegraphDuration * amount,
            CommitDuration * amount,
            ImpactDuration * amount,
            RecoveryDuration * amount
        );
    }
}
