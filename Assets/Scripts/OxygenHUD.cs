using UnityEngine;
using UnityEngine.UI;

// Drives the oxygen vignette — a red Image overlay that pulses at low oxygen.
public class OxygenHUD : MonoBehaviour
{
    public Image  vignetteOverlay;
    public float  pulseSpeed = 2f;

    [Header("Thresholds")]
    public float warningThreshold = 0.5f;
    public float panicThreshold   = 0.25f;

    void Update()
    {
        if (vignetteOverlay == null || OxygenSystem.Instance == null) return;

        float o = OxygenSystem.Instance.NormalizedLeft;
        Color c = vignetteOverlay.color;

        if (o > warningThreshold)
        {
            c.a = 0f;
        }
        else if (o > panicThreshold)
        {
            c.a = Mathf.InverseLerp(warningThreshold, panicThreshold, o) * 0.3f;
        }
        else
        {
            // Pulse at panic level
            float pulse = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f;
            c.a = Mathf.Lerp(0.2f, 0.6f, pulse);
        }

        vignetteOverlay.color = c;
    }
}
