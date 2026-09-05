using UnityEngine;

// Attach to the Star mesh (child of SignalEmitter object).
// Star is invisible beyond fadeRange units; fades in to full alpha as player gets closer.
[RequireComponent(typeof(Renderer))]
public class StarFade : MonoBehaviour
{
    [Tooltip("Distance in units at which the star starts becoming visible")]
    public float fadeRange = 30f;

    Renderer              _renderer;
    MaterialPropertyBlock _mpb;
    SignalEmitter         _emitter;
    Color                 _baseColor;
    Color                 _emission;

    void Awake()
    {
        _renderer  = GetComponent<Renderer>();
        _mpb       = new MaterialPropertyBlock();
        _emitter   = GetComponentInParent<SignalEmitter>();

        _baseColor = _renderer.sharedMaterial.GetColor("_BaseColor");
        _emission  = _renderer.sharedMaterial.GetColor("_EmissionColor");

        // Hidden at start
        _renderer.enabled = false;
    }

    void Update()
    {
        if (_emitter == null) return;

        if (Camera.main != null)
            transform.rotation = Camera.main.transform.rotation;

        // Convert normalizedDistance back to actual distance in units
        float dist = _emitter.normalizedDistance * _emitter.maxRange;
        // t = 0 when dist >= fadeRange, t = 1 when right at the star
        float t = 1f - Mathf.Clamp01(dist / fadeRange);

        if (t <= 0f)
        {
            _renderer.enabled = false;
            return;
        }

        _renderer.enabled = true;
        Apply(t);
    }

    void Apply(float t)
    {
        _renderer.GetPropertyBlock(_mpb);
        _mpb.SetColor("_BaseColor",     new Color(_baseColor.r, _baseColor.g, _baseColor.b, t));
        _mpb.SetColor("_EmissionColor", _emission * t);
        _renderer.SetPropertyBlock(_mpb);
    }
}
