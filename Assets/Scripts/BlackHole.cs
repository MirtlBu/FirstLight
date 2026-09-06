using UnityEngine;

// Attach to the black_hole GameObject (or its parent).
// - Fades in renderer alpha as player approaches
// - Fades in alarm AudioSource by proximity
[RequireComponent(typeof(AudioSource))]
public class BlackHole : MonoBehaviour
{
    [Header("Visual")]
    public Renderer visual;          // assign the black hole mesh/quad renderer
    public float    fadeRange  = 80f;  // distance at which it starts becoming visible
    public float    fadeSpeed  = 0.5f;

    [Header("Danger Audio")]
    public float    audioRange = 120f; // distance at which alarm starts
    [Range(0f, 1f)]
    public float    maxVolume  = 0.9f;
    public float    audioFadeSpeed = 0.4f;

    AudioSource           _audio;
    MaterialPropertyBlock _mpb;
    Color                 _baseColor;
    Color                 _emission;
    float                 _currentAlpha;
    bool                  _hasBaseColor;
    bool                  _hasEmission;

    void Awake()
    {
        _audio = GetComponent<AudioSource>();
        _audio.volume = 0f;
        _audio.loop   = true;
        if (!_audio.isPlaying) _audio.Play();

        if (visual != null)
        {
            _mpb = new MaterialPropertyBlock();
            var mat = visual.sharedMaterial;
            if (mat != null)
            {
                _hasBaseColor = mat.HasProperty("_BaseColor");
                _hasEmission  = mat.HasProperty("_EmissionColor");
                if (_hasBaseColor) _baseColor = mat.GetColor("_BaseColor");
                if (_hasEmission)  _emission  = mat.GetColor("_EmissionColor");
            }
            ApplyAlpha(0f);
        }
    }

    void Update()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.transform.position);

        UpdateVisual(dist);
        UpdateAudio(dist);
    }

    void UpdateVisual(float dist)
    {
        if (visual == null) return;

        float target = 1f - Mathf.Clamp01(dist / fadeRange);
        _currentAlpha = Mathf.MoveTowards(_currentAlpha, target, fadeSpeed * Time.deltaTime);

        visual.enabled = _currentAlpha > 0f;
        if (visual.enabled)
            ApplyAlpha(_currentAlpha);
    }

    void UpdateAudio(float dist)
    {
        float target = (1f - Mathf.Clamp01(dist / audioRange)) * maxVolume;
        _audio.volume = Mathf.MoveTowards(_audio.volume, target, audioFadeSpeed * Time.deltaTime);
    }

    void ApplyAlpha(float a)
    {
        visual.GetPropertyBlock(_mpb);
        if (_hasBaseColor)
            _mpb.SetColor("_BaseColor", new Color(_baseColor.r, _baseColor.g, _baseColor.b, a));
        if (_hasEmission)
            _mpb.SetColor("_EmissionColor", _emission * a);
        visual.SetPropertyBlock(_mpb);
    }
}
