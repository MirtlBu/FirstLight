using UnityEngine;

// Attach to a particle system inside a SignalEmitter object.
// Particles are invisible beyond fadeRange units; fade in as player gets closer.
[RequireComponent(typeof(ParticleSystem))]
public class GlowParticles : MonoBehaviour
{
    [Tooltip("Distance in units at which particles start becoming visible")]
    public float fadeRange = 30f;

    ParticleSystem         _ps;
    ParticleSystemRenderer _renderer;
    MaterialPropertyBlock  _mpb;
    SignalEmitter          _emitter;
    Color                  _matColor;
    Color                  _emission;

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

        float dist = _emitter.normalizedDistance * _emitter.maxRange;
        float t    = 1f - Mathf.Clamp01(dist / fadeRange);

        if (t <= 0f)
        {
            if (_ps.isEmitting)
                _ps.Stop(false, ParticleSystemStopBehavior.StopEmitting);
            ApplyAlpha(0f);
            return;
        }

        if (!_ps.isPlaying) _ps.Play(true);
        ApplyAlpha(t);
    }

    void ApplyAlpha(float t)
    {
        _renderer.GetPropertyBlock(_mpb);
        _mpb.SetColor("_BaseColor",     new Color(_matColor.r, _matColor.g, _matColor.b, _matColor.a * t));
        _mpb.SetColor("_EmissionColor", _emission * t);
        _renderer.SetPropertyBlock(_mpb);
    }
}
