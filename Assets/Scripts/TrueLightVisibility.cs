using UnityEngine;

public class TrueLightVisibility : MonoBehaviour
{
    [Header("Visibility")]
    public Transform player;
    public float visibleDistance = 5f;
    public Renderer[] visibleRenderers;
    public GameObject[] visibleEffects;
    public float fadeSpeed = 6f;

    Renderer[] _renderers;
    MaterialPropertyBlock _propertyBlock;
    Color[] _baseColors;
    Color[] _baseEmissions;
    float _visibility;

    void Awake()
    {
        _propertyBlock = new MaterialPropertyBlock();
        _renderers = visibleRenderers != null && visibleRenderers.Length > 0
            ? visibleRenderers
            : GetComponentsInChildren<Renderer>();
        _baseColors = new Color[_renderers.Length];
        _baseEmissions = new Color[_renderers.Length];

        for (int i = 0; i < _renderers.Length; i++)
        {
            Material material = _renderers[i].sharedMaterial;
            _baseColors[i] = material != null ? material.GetColor("_BaseColor") : Color.white;
            _baseEmissions[i] = material != null ? material.GetColor("_EmissionColor") : Color.white;
        }

        SetEffects(false);
    }

    void Update()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindWithTag("Player");
            if (playerObject != null) player = playerObject.transform;
        }

        float target = player != null && Vector3.Distance(transform.position, player.position) <= visibleDistance ? 1f : 0f;
        _visibility = Mathf.MoveTowards(_visibility, target, fadeSpeed * Time.deltaTime);
        ApplyVisibility(_visibility);
    }

    void ApplyVisibility(float amount)
    {
        bool visible = amount > 0.001f;
        SetEffects(visible);

        for (int i = 0; i < _renderers.Length; i++)
        {
            if (_renderers[i] == null) continue;
            _renderers[i].enabled = visible;
            _renderers[i].GetPropertyBlock(_propertyBlock);
            Color baseColor = _baseColors[i];
            _propertyBlock.SetColor("_BaseColor", new Color(baseColor.r, baseColor.g, baseColor.b, baseColor.a * amount));
            _propertyBlock.SetColor("_EmissionColor", _baseEmissions[i] * amount);
            _renderers[i].SetPropertyBlock(_propertyBlock);
        }
    }

    void SetEffects(bool active)
    {
        if (visibleEffects == null) return;
        foreach (GameObject effect in visibleEffects)
            if (effect != null) effect.SetActive(active);
    }
}
