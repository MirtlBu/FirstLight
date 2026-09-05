using UnityEngine;
using UnityEngine.Rendering;

public class AmbientLightController : MonoBehaviour
{
    [Header("Colors")]
    public Color coldColor = new Color(0.02f, 0.02f, 0.06f); // deep cold blue-black
    public Color warmColor = new Color(0.6f,  0.3f,  0.05f); // amber

    [Header("Camera Background")]
    public Color coldBg = new Color(0f, 0f, 0f);
    public Color warmBg = new Color(0.08f, 0.03f, 0f); // very faint amber tint

    Camera _cam;

    void Awake()
    {
        RenderSettings.ambientMode = AmbientMode.Flat;
        _cam = Camera.main;
    }

    public void SetIntensity(float t)
    {
        RenderSettings.ambientLight = Color.Lerp(coldColor, warmColor, t);
        if (_cam != null)
            _cam.backgroundColor = Color.Lerp(coldBg, warmBg, t);
    }
}
