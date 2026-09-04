using UnityEngine;
using UnityEngine.UI;

// Controls a full-screen UI overlay that represents visor frost.
// frost alpha = 1 when far, 0 when close to real star.
public class VisorFrostController : MonoBehaviour
{
    public Image frostOverlay;

    public void SetMelt(float melt)
    {
        if (frostOverlay == null) return;
        Color c = frostOverlay.color;
        c.a = Mathf.Lerp(c.a, 1f - melt, Time.deltaTime * 1.5f);
        frostOverlay.color = c;
    }
}
