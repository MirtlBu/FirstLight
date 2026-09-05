using UnityEngine;

// Attach to a SignalEmitter object (or its child).
// Fades in an AudioSource as the player approaches within fadeRange.
// For ambient background noise, add a separate AudioSource on the Camera with Loop enabled.
[RequireComponent(typeof(AudioSource))]
public class ProximityAudio : MonoBehaviour
{
    [Tooltip("Distance in units at which the sound starts to fade in")]
    public float fadeRange = 80f;

    [Tooltip("Maximum volume when right at the source")]
    [Range(0f, 1f)]
    public float maxVolume = 0.8f;

    [Tooltip("How fast volume changes (units per second)")]
    public float fadeSpeed = 0.5f;

    AudioSource    _audio;
    SignalEmitter  _emitter;

    void Awake()
    {
        _audio   = GetComponent<AudioSource>();
        _emitter = GetComponentInParent<SignalEmitter>();
        if (_emitter == null)
            _emitter = GetComponent<SignalEmitter>();

        _audio.volume = 0f;
        _audio.loop   = true;
        if (!_audio.isPlaying)
            _audio.Play();
    }

    void Update()
    {
        if (_emitter == null) return;

        float dist   = _emitter.normalizedDistance * _emitter.maxRange;
        float target = (1f - Mathf.Clamp01(dist / fadeRange)) * maxVolume;

        _audio.volume = Mathf.MoveTowards(_audio.volume, target, fadeSpeed * Time.deltaTime);
    }
}
