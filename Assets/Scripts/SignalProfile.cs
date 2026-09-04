using UnityEngine;

// Defines which signal channels an object emits.
// True = this object correctly emits that signal (like the real star).
// False = this channel is silent / misleading for this object.
[System.Serializable]
public class SignalProfile
{
    [Header("Layer 1 — Always On")]
    public bool colorTemperature = true;
    public bool breathingEase    = true;
    public bool musicTone        = true;

    [Header("Layer 2 — Mid Range")]
    public bool horizonGlow      = true;
    public bool stardustFlow     = true;
    public bool hullResonance    = true;
    public bool geigerPing       = true;

    [Header("Layer 3 — Close Only")]
    public bool visorFrostMelt   = true;
    public bool shadowAppearance = true;
    public bool radioMelody      = true;
    public bool heartbeat        = true;

    // Returns a 0-1 intensity for a given channel, factoring distance and layer activation.
    public float GetIntensity(bool channel, float normalizedDistance)
    {
        if (!channel) return 0f;
        return 1f - normalizedDistance;
    }
}
