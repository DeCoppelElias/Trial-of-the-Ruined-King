using UnityEngine;
using UnityEngine.Audio;

public enum AudioPriority
{
    Low,
    Normal,
    High,
    Critical
}

[CreateAssetMenu(menuName = "Audio/Audio Event")]
public class AudioEvent : ScriptableObject
{
    [Header("Clips")]
    public AudioClip[] clips;

    [Header("Playback")]
    [Range(0f, 1f)] public float volume = 1f;
    public Vector2 pitchRange = new Vector2(0.97f, 1.03f);

    [Header("Rules")]
    public AudioPriority priority = AudioPriority.Normal;
    public float cooldown = 0f;

    [Header("Routing")]
    public AudioMixerGroup mixerGroup;

    [Header("Pooling")]
    public int poolSize = 3;
}
