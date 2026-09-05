using UnityEngine;

// Attach to a particle system inside a SignalEmitter object.
// Particles are invisible beyond fadeRange units; fade in as player gets closer.
[RequireComponent(typeof(ParticleSystem))]
public class GlowParticles : MonoBehaviour
{
    [Tooltip("Distance in units at which particles start becoming visible")]
    public float fadeRange = 30f;
    [Tooltip("Maximum alpha the particles reach when player is right at the object")]
    [Range(0f, 1f)]
    public float maxAlpha  = 0.5f;
    [Tooltip("How fast alpha changes (units per second). Lower = slower fade")]
    public float fadeSpeed = 0.15f;

    ParticleSystem         _ps;
    ParticleSystemRenderer _renderer;
    MaterialPropertyBlock  _mpb;
    SignalEmitter          _emitter;
    Color                  _matColor;
    Color                  _emission;
    float                  _currentAlpha;

    void Awake()
    {
        _ps       = GetComponent<ParticleSystem>();
        _renderer = GetComponent<ParticleSystemRenderer>();
        _mpb      = new MaterialPropertyBlock();
        _emitter  = GetComponentInParent<SignalEmitter>();

        if (_renderer.sharedMaterial != null)
        {
            _matColor = _renderer.sharedMaterial.GetColor("_BaseColor");
            _emission = _renderer.sharedMaterial.GetColor("_EmissionColor");
        }

        _ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        ApplyAlpha(0f);
    }

    void Update()
    {
        if (_emitter == null) return;

        float dist   = _emitter.normalizedDistance * _emitter.maxRange;
        float target = (1f - Mathf.Clamp01(dist / fadeRange)) * maxAlpha;

        _currentAlpha = Mathf.MoveTowards(_currentAlpha, target, fadeSpeed * Time.deltaTime);

        if (_currentAlpha <= 0f)
        {
            if (_ps.isEmitting)
                _ps.Stop(false, ParticleSystemStopBehavior.StopEmitting);
            ApplyAlpha(0f);
            return;
        }

        if (!_ps.isPlaying) _ps.Play(true);
        ApplyAlpha(_currentAlpha);
    }

    void ApplyAlpha(float a)
    {
        _renderer.GetPropertyBlock(_mpb);
        _mpb.SetColor("_BaseColor",     new Color(_matColor.r, _matColor.g, _matColor.b, _matColor.a * a));
        _mpb.SetColor("_EmissionColor", _emission * a);
        _renderer.SetPropertyBlock(_mpb);
    }
}
