using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BeamReflectionTarget : MonoBehaviour
{
    [Header("Reflection")]
    public Renderer[] reflectionRenderers;
    public Color reflectionColor = new Color(1f, 0.85f, 0.55f, 1f);
    public float reflectionIntensity = 1.5f;
    public float fadeSpeed = 5f;
    public bool blockBeam = true;
    public float glintRange = 0.12f;

    [Header("Sound")]
    public AudioClip reflectionClip;
    [Range(0f, 1f)] public float reflectionVolume = 0.65f;
    public float soundCooldown = 0.5f;

    AudioSource _audioSource;
    Light _glintLight;
    float _brightness;
    float _lastSoundTime = -Mathf.Infinity;
    bool _beamHit;

    void Awake()
    {
        if (reflectionRenderers == null || reflectionRenderers.Length == 0)
            reflectionRenderers = GetComponentsInChildren<Renderer>();

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

        GameObject glintObject = new GameObject("BeamGlint");
        glintObject.transform.SetParent(transform, true);
        _glintLight = glintObject.AddComponent<Light>();
        _glintLight.type = LightType.Spot;
        _glintLight.color = reflectionColor;
        _glintLight.range = glintRange;
        _glintLight.spotAngle = 8f;
        _glintLight.innerSpotAngle = 4f;
        _glintLight.intensity = 0f;
        _glintLight.shadows = LightShadows.Hard;
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

    public void SetBeamHit(bool hit, RaycastHit hitInfo)
    {
        if (hit && !_beamHit && Time.time >= _lastSoundTime + soundCooldown)
        {
            if (_audioSource != null && reflectionClip != null)
                _audioSource.PlayOneShot(reflectionClip, reflectionVolume);
            _lastSoundTime = Time.time;
        }

        _beamHit = hit;
        if (hit && _glintLight != null)
        {
            _glintLight.transform.position = hitInfo.point + hitInfo.normal * 0.04f;
            _glintLight.transform.rotation = Quaternion.LookRotation(-hitInfo.normal, Vector3.up);
        }
    }

    void Update()
    {
        float targetBrightness = _beamHit ? 1f : 0f;
        _brightness = Mathf.MoveTowards(_brightness, targetBrightness, fadeSpeed * Time.deltaTime);

        if (_glintLight != null)
            _glintLight.intensity = reflectionIntensity * _brightness;
    }
}
