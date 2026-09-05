using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BeamReflectionTarget : MonoBehaviour
{
    [Header("Reflection")]
    public Renderer[] reflectionRenderers;
    public Color reflectionColor = new Color(1f, 0.85f, 0.55f, 1f);
    public float reflectionIntensity = 4f;
    public float fadeSpeed = 5f;
    public bool blockBeam = true;

    [Header("Sound")]
    public AudioClip reflectionClip;
    [Range(0f, 1f)] public float reflectionVolume = 0.65f;
    public float soundCooldown = 0.5f;

    MaterialPropertyBlock _propertyBlock;
    Color[] _baseEmissions;
    AudioSource _audioSource;
    float _brightness;
    float _lastSoundTime = -Mathf.Infinity;
    bool _beamHit;

    void Awake()
    {
        _propertyBlock = new MaterialPropertyBlock();
        if (reflectionRenderers == null || reflectionRenderers.Length == 0)
            reflectionRenderers = GetComponentsInChildren<Renderer>();

        _baseEmissions = new Color[reflectionRenderers.Length];
        for (int i = 0; i < reflectionRenderers.Length; i++)
        {
            Material material = reflectionRenderers[i].sharedMaterial;
            _baseEmissions[i] = material != null ? material.GetColor("_EmissionColor") : Color.black;
        }

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null && reflectionClip != null)
            _audioSource = gameObject.AddComponent<AudioSource>();
        if (_audioSource != null)
        {
            _audioSource.playOnAwake = false;
            _audioSource.spatialBlend = 1f;
            _audioSource.minDistance = 1f;
            _audioSource.maxDistance = 60f;
            _audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        }
    }

    void OnEnable()
    {
        if (!LighthouseBeam.Targets.Contains(this))
            LighthouseBeam.Targets.Add(this);
    }

    void OnDisable()
    {
        LighthouseBeam.Targets.Remove(this);
    }

    public void SetBeamHit(bool hit)
    {
        if (hit && !_beamHit && Time.time >= _lastSoundTime + soundCooldown)
        {
            if (_audioSource != null && reflectionClip != null)
                _audioSource.PlayOneShot(reflectionClip, reflectionVolume);
            _lastSoundTime = Time.time;
        }

        _beamHit = hit;
    }

    void Update()
    {
        float targetBrightness = _beamHit ? 1f : 0f;
        _brightness = Mathf.MoveTowards(_brightness, targetBrightness, fadeSpeed * Time.deltaTime);

        for (int i = 0; i < reflectionRenderers.Length; i++)
        {
            Renderer renderer = reflectionRenderers[i];
            if (renderer == null) continue;

            renderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetColor("_EmissionColor", _baseEmissions[i] + reflectionColor * (reflectionIntensity * _brightness));
            renderer.SetPropertyBlock(_propertyBlock);
        }
    }
}
