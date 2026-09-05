using UnityEngine;

// Attach to a large transparent sphere child of a SignalEmitter object.
// Alpha fades in as the player gets closer.
[RequireComponent(typeof(Renderer))]
public class GlowSphere : MonoBehaviour
{
    [Header("Glow")]
    public Color glowColor = new Color(1f, 0.6f, 0.1f, 1f); // set per object
    public float maxAlpha  = 0.35f;
    public AnimationCurve alphaCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    Renderer          _renderer;
    MaterialPropertyBlock _mpb;
    SignalEmitter     _emitter;

    void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _mpb      = new MaterialPropertyBlock();
        _emitter  = GetComponentInParent<SignalEmitter>();
    }

    void Update()
    {
        if (_emitter == null) return;

        // normalizedDistance: 0 = at emitter, 1 = at maxRange
        float proximity = 1f - _emitter.normalizedDistance; // 0 = far, 1 = close
        float alpha     = alphaCurve.Evaluate(proximity) * maxAlpha;

        _renderer.GetPropertyBlock(_mpb);
        _mpb.SetColor("_BaseColor", new Color(glowColor.r, glowColor.g, glowColor.b, alpha));
        _renderer.SetPropertyBlock(_mpb);
    }
}
