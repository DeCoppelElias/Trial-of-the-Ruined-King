using System.Collections.Generic;
using UnityEngine;
public class SpearColumnPresentation : SpearPresentationBase
{
    private readonly VerticalDirection _direction;

    public SpearColumnPresentation(
        Attack attack,
        IArena arena,
        Transform parent,
        TileVisualizer tileVisualizer,
        AudioPlayer audioPlayer,
        SpearVisualConfig visualConfig,
        SpearAudioConfig audioConfig)
        : base(attack, arena, parent, tileVisualizer, audioPlayer, visualConfig, audioConfig)
    {
        SpearColumnAttack columnAttack = attack as SpearColumnAttack;
        _direction = columnAttack != null ? columnAttack.Direction : VerticalDirection.BottomToTop;
        CalculateSpearTrajectory(attack);
    }

    protected override void SortPositions(List<GridPosition> positions)
    {
        if (_direction == VerticalDirection.BottomToTop)
        {
            positions.Sort((a, b) => a.GridY.CompareTo(b.GridY));
        }
        else
        {
            positions.Sort((a, b) => b.GridY.CompareTo(a.GridY)); 
        }
    }

    protected override void DetermineStartAndEndPositions(List<GridPosition> sortedPositions, out GridPosition startPos, out GridPosition endPos)
    {
        startPos = sortedPositions[0];
        endPos = sortedPositions[sortedPositions.Count - 1];
    }

    protected override Quaternion CalculateSpearRotation(Vector3 direction)
    {
        float zRotation = direction.z > 0 ? 0f : 180f;
        return Quaternion.Euler(90f, 0f, zRotation);
    }

    protected override string GetSpearInstanceName()
    {
        return "SpearProjectile_Column";
    }
}