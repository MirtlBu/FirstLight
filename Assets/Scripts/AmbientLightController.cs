using UnityEngine;

public class AmbientLightController : MonoBehaviour
{
    [Header("Colors")]
    public Color coldColor = new Color(0.02f, 0.02f, 0.06f); // deep cold blue-black
    public Color warmColor = new Color(0.6f,  0.3f,  0.05f); // amber

    public void SetIntensity(float t)
    {
        RenderSettings.ambientLight = Color.Lerp(coldColor, warmColor, t);
    }
}
