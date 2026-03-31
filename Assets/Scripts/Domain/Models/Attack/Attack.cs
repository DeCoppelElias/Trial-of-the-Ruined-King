using System;
using System.Collections.Generic;
using System.Linq;

public abstract class Attack
{
    public AttackStage CurrentStage { get; private set; }
    public float ElapsedTimeInStage { get; private set; }
    public AttackTiming Timing { get; private set; }
    public IAttackPattern Pattern { get; private set; }
    public bool HasDealtDamage { get; private set; }
    
    public event Action<Attack, AttackStage> OnStageChanged;
    public event Action OnAttackComplete;

    protected Attack(AttackTiming timing, IAttackPattern pattern)
    {
        Timing = timing;
        Pattern = pattern;
        CurrentStage = AttackStage.None;
        ElapsedTimeInStage = 0f;
        HasDealtDamage = false;
    }

    public void MarkDamageDealt()
    {
        HasDealtDamage = true;
    }

    public virtual bool IsPositionUnderAttack(GridPosition position)
    {
        var state = GetCurrentState();
        return Pattern.GetActiveDangerPositions(state).Any(pos => pos.Equals(position));
    }

    public AttackState GetCurrentState()
    {
        float progress = GetStageProgress();
        return new AttackState(CurrentStage, progress);
    }

    public void Start()
    {
        if (CurrentStage != AttackStage.None) return;

        CurrentStage = AttackStage.Telegraph;
        ElapsedTimeInStage = 0f;
        OnStageChanged?.Invoke(this, CurrentStage);
    }

    public virtual void OnTick(float deltaTime)
    {
        if (CurrentStage == AttackStage.None || CurrentStage == AttackStage.Complete) return;

        ElapsedTimeInStage += deltaTime;
        
        float stageDuration = Timing.GetDurationForStage(CurrentStage);
        
        if (ElapsedTimeInStage >= stageDuration)
        {
            AdvanceToNextStage();
        }
    }

    private void AdvanceToNextStage()
    {
        ElapsedTimeInStage = 0f;
        
        switch (CurrentStage)
        {
            case AttackStage.Telegraph:
                CurrentStage = AttackStage.Commit;
                break;
            case AttackStage.Commit:
                CurrentStage = AttackStage.Impact;
                break;
            case AttackStage.Impact:
                CurrentStage = AttackStage.Recovery;
                break;
            case AttackStage.Recovery:
                CurrentStage = AttackStage.Complete;
                OnAttackComplete?.Invoke();
                return;
        }
        
        OnStageChanged?.Invoke(this, CurrentStage);
    }

    public bool IsComplete()
    {
        return CurrentStage == AttackStage.Complete;
    }

    public float GetStageProgress()
    {
        float stageDuration = Timing.GetDurationForStage(CurrentStage);
        if (stageDuration <= 0f) return 1f;
        return ElapsedTimeInStage / stageDuration;
    }
}
