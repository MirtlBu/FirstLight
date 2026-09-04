using UnityEngine;

// Controls two AudioSources: calmBreath and anxiousBreath.
// ease = 0 (far from star), 1 (at star).
// oxygen = 1 (full), 0 (empty).
public class BreathingController : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource calmBreath;
    public AudioSource anxiousBreath;

    [Header("Oxygen thresholds")]
    public float panicThreshold = 0.25f;

    public void SetEase(float ease, float oxygen)
    {
        // Proximity drives calm breathing
        float calmVol = ease;

        // Low oxygen drives anxious breathing (overrides proximity)
        float oxygenPanic = Mathf.InverseLerp(panicThreshold, 0.5f, oxygen);
        float anxiousVol  = Mathf.Max(1f - ease, oxygenPanic);

        if (calmBreath    != null) calmBreath.volume    = Mathf.Lerp(calmBreath.volume,    calmVol,    Time.deltaTime * 2f);
        if (anxiousBreath != null) anxiousBreath.volume = Mathf.Lerp(anxiousBreath.volume, anxiousVol, Time.deltaTime * 2f);
    }
}
