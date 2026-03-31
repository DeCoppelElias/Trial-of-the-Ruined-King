using System.Collections.Generic;
using UnityEngine;
public class SpearRowPresentation : SpearPresentationBase
{
    private readonly HorizontalDirection _direction;

    public SpearRowPresentation(
        Attack attack,
        IArena arena,
        Transform parent,
        TileVisualizer tileVisualizer,
        AudioPlayer audioPlayer,
        SpearVisualConfig visualConfig,
        SpearAudioConfig audioConfig)
        : base(attack, arena, parent, tileVisualizer, audioPlayer, visualConfig, audioConfig)
    {
        SpearRowAttack rowAttack = attack as SpearRowAttack;
        _direction = rowAttack != null ? rowAttack.Direction : HorizontalDirection.LeftToRight;
        CalculateSpearTrajectory(attack);
    }

    protected override void SortPositions(List<GridPosition> positions)
    {
        if (_direction == HorizontalDirection.LeftToRight)
        {
            positions.Sort((a, b) => a.GridX.CompareTo(b.GridX));
        }
        else
        {
            positions.Sort((a, b) => b.GridX.CompareTo(a.GridX));
        }
    }

    protected override void DetermineStartAndEndPositions(List<GridPosition> sortedPositions, out GridPosition startPos, out GridPosition endPos)
    {
        startPos = sortedPositions[0];
        endPos = sortedPositions[sortedPositions.Count - 1];
    }

    protected override Quaternion CalculateSpearRotation(Vector3 direction)
    {
        float zRotation = direction.x > 0 ? -90f : 90f;
        return Quaternion.Euler(90f, 0f, zRotation);
    }

    protected override string GetSpearInstanceName()
    {
        return "SpearProjectile_Row";
    }
}