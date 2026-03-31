public struct AttackState
{
    public AttackStage Stage { get; }
    public float ProgressInStage { get; }

    public AttackState(AttackStage stage, float progressInStage)
    {
        Stage = stage;
        ProgressInStage = progressInStage;
    }
}
