using System;
using UnityEngine;

[Serializable]
public abstract class AttackStepParametersBase
{
    // Base class can have common properties if needed in the future
}

[Serializable]
public class SpearRowParameters : AttackStepParametersBase
{
    public int row;
    public HorizontalDirection rowDirection;
}

[Serializable]
public class SpearColumnParameters : AttackStepParametersBase
{
    public int column;
    public VerticalDirection columnDirection;
}

[Serializable]
public class HammerParameters : AttackStepParametersBase
{
    public GridPosition hammerPosition;
    public HorizontalDirection hammerDirection;
}
