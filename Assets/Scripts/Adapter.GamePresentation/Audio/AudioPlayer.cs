using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private bool logSuppressedSounds = false;

    private readonly Dictionary<AudioEvent, float> _lastPlayTimes = new();
    private readonly Dictionary<AudioEvent, Queue<AudioSource>> _pools = new();

    public void Play(AudioEvent audioEvent)
    {
        if (audioEvent == null)
            return;

        if (!CanPlay(audioEvent))
            return;

        AudioSource source = GetSource(audioEvent);
        ConfigureSource(source, audioEvent);
        source.Play();

        _lastPlayTimes[audioEvent] = Time.time;
    }

    private bool CanPlay(AudioEvent audioEvent)
    {
        if (audioEvent.cooldown <= 0f)
            return true;

        if (_lastPlayTimes.TryGetValue(audioEvent, out float lastTime))
        {
            if (Time.time - lastTime < audioEvent.cooldown)
            {
                if (logSuppressedSounds)
                    Debug.Log($"[AudioPlayer] Suppressed {audioEvent.name} (cooldown)");

                return false;
            }
        }

        return true;
    }

    private AudioSource GetSource(AudioEvent audioEvent)
    {
        if (!_pools.TryGetValue(audioEvent, out Queue<AudioSource> pool))
        {
            Debug.Log($"[AudioPlayer] Creating pool for {audioEvent.name} (size: {audioEvent.poolSize})");
            pool = CreatePool(audioEvent);
            _pools[audioEvent] = pool;
        }

        AudioSource source = pool.Dequeue();
        pool.Enqueue(source);
        return source;
    }

    private Queue<AudioSource> CreatePool(AudioEvent audioEvent)
    {
        var pool = new Queue<AudioSource>();

        for (int i = 0; i < audioEvent.poolSize; i++)
        {
            var go = new GameObject($"AudioSource_{audioEvent.name}_{i}");
            go.transform.SetParent(transform);

            var source = go.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = 0f;

            pool.Enqueue(source);
        }

        return pool;
    }

    private void ConfigureSource(AudioSource source, AudioEvent audioEvent)
    {
        source.clip = PickClip(audioEvent);
        source.volume = audioEvent.volume;
        source.pitch = Random.Range(
            audioEvent.pitchRange.x,
            audioEvent.pitchRange.y
        );
        source.outputAudioMixerGroup = audioEvent.mixerGroup;
    }

    private AudioClip PickClip(AudioEvent audioEvent)
    {
        if (audioEvent.clips == null || audioEvent.clips.Length == 0)
            return null;

        if (audioEvent.clips.Length == 1)
            return audioEvent.clips[0];

        return audioEvent.clips[Random.Range(0, audioEvent.clips.Length)];
    }
}
