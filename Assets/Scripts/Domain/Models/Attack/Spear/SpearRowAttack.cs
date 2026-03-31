using UnityEngine;

public class SpearRowAttack : Attack
{
    public int TargetRow { get; private set; }
    public HorizontalDirection Direction { get; private set; }

    public SpearRowAttack(int targetRow, HorizontalDirection direction, AttackTiming timing, IArena arena) 
        : base(timing, new LinearProgressivePattern(targetRow, direction, arena))
    {
        TargetRow = targetRow;
        Direction = direction;
    }
}
