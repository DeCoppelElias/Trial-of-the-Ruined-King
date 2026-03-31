using Codice.Client.Common;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpearColumnAttack : Attack
{
    public int TargetColumn { get; private set; }
    public VerticalDirection Direction { get; private set; }

    public SpearColumnAttack(int targetColumn, VerticalDirection direction, AttackTiming timing, IArena arena)
        : base(timing, new LinearProgressivePattern(targetColumn, direction, arena))
    {
        TargetColumn = targetColumn;
        Direction = direction;
    }
}