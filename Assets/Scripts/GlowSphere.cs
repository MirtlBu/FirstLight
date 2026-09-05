using UnityEngine;

// Attach to a Quad/Plane child of a SignalEmitter object.
// Billboard mode keeps it always facing the camera — no visible edges.
// Alpha and emission fade in as player enters fadeRange.
[RequireComponent(typeof(Renderer))]
public class GlowSphere : MonoBehaviour
{
    [Tooltip("Distance in units at which glow starts appearing")]
    public float fadeRange = 60f;
    [Range(0f, 1f)]
    public float maxAlpha  = 0.35f;
    [Tooltip("How fast alpha changes (units per second). Lower = slower fade")]
    public float fadeSpeed = 0.1f;
    [Tooltip("Always rotate to face the camera (use with Quad/Plane mesh)")]
    public bool  billboard = true;

    Renderer              _renderer;
    MaterialPropertyBlock _mpb;
    SignalEmitter         _emitter;
    Color                 _baseColor;
    Color                 _emission;
    float                 _currentAlpha;

    void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _mpb      = new MaterialPropertyBlock();
        _emitter  = GetComponentInParent<SignalEmitter>();

        if (_renderer.sharedMaterial != null)
        {
            _baseColor = _renderer.sharedMaterial.GetColor("_BaseColor");
            _emission  = _renderer.sharedMaterial.GetColor("_EmissionColor");
        }

        _renderer.enabled = false;
    }

    void Update()
    {
        if (_emitter == null) return;

        // Billboard: always face the camera
        if (billboard && Camera.main != null)
            transform.rotation = Camera.main.transform.rotation;

        float dist   = _emitter.normalizedDistance * _emitter.maxRange;
        float target = (1f - Mathf.Clamp01(dist / fadeRange)) * maxAlpha;

        _currentAlpha = Mathf.MoveTowards(_currentAlpha, target, fadeSpeed * Time.deltaTime);

        if (_currentAlpha <= 0f)
        {
            _renderer.enabled = false;
            return;
        }

        _renderer.enabled = true;
        _renderer.GetPropertyBlock(_mpb);
        _mpb.SetColor("_BaseColor",     new Color(_baseColor.r, _baseColor.g, _baseColor.b, _currentAlpha));
        _mpb.SetColor("_EmissionColor", _emission * _currentAlpha);
        _renderer.SetPropertyBlock(_mpb);
    }
}
