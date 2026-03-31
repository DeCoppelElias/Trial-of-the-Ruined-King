using UnityEngine;
[System.Serializable]
public class HammerVisualConfig
{
    public GameObject HammerPrefab;
    public Vector3 InitialScale;
    public Vector3 FinalScale;

    public float ArenaShakeDuration;
    public float ArenaShakeMagnitude;

    public float TelegraphStartOffsetX;
    public float TelegraphEndOffsetX;
    public float TelegraphOffsetY;
}