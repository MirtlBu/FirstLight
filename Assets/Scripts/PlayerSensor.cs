using UnityEngine;
using System.Collections.Generic;

// Finds the nearest SignalEmitter and exposes per-channel intensity for FeedbackOrchestrator.
public class PlayerSensor : MonoBehaviour
{
    public static PlayerSensor Instance { get; private set; }

    // Per-channel output, 0-1. Read by FeedbackOrchestrator.
    [HideInInspector] public float colorTemperature;
    [HideInInspector] public float breathingEase;
    [HideInInspector] public float musicTone;
    [HideInInspector] public float horizonGlow;
    [HideInInspector] public float stardustFlow;
    [HideInInspector] public float hullResonance;
    [HideInInspector] public float geigerPing;
    [HideInInspector] public float visorFrostMelt;
    [HideInInspector] public float shadowAppearance;
    [HideInInspector] public float radioMelody;
    [HideInInspector] public float heartbeat;

    [HideInInspector] public SignalEmitter nearest;
    [HideInInspector] public float         oxygenDrainFromNearest;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        SignalEmitter[] emitters = FindObjectsByType<SignalEmitter>();
        float           best     = float.MaxValue;
        nearest = null;

        foreach (var e in emitters)
        {
            if (e.normalizedDistance * e.maxRange < best)
            {
                best    = e.normalizedDistance * e.maxRange;
                nearest = e;
            }
        }

        if (nearest == null)
        {
            ResetAll();
            return;
        }

        SignalProfile p    = nearest.profile;
        float         dist = nearest.normalizedDistance;

        colorTemperature = p.GetIntensity(p.colorTemperature, dist);
        breathingEase    = p.GetIntensity(p.breathingEase,    dist);
        musicTone        = p.GetIntensity(p.musicTone,        dist);
        horizonGlow      = p.GetIntensity(p.horizonGlow,      dist);
        stardustFlow     = p.GetIntensity(p.stardustFlow,     dist);
        hullResonance    = p.GetIntensity(p.hullResonance,    dist);
        geigerPing       = p.GetIntensity(p.geigerPing,       dist);
        visorFrostMelt   = p.GetIntensity(p.visorFrostMelt,   dist);
        shadowAppearance = p.GetIntensity(p.shadowAppearance, dist);
        radioMelody      = p.GetIntensity(p.radioMelody,      dist);
        heartbeat        = p.GetIntensity(p.heartbeat,        dist);

        oxygenDrainFromNearest = nearest.oxygenDrainPerSecond * (1f - dist);
    }

    void ResetAll()
    {
        colorTemperature = breathingEase = musicTone = horizonGlow = stardustFlow =
        hullResonance = geigerPing = visorFrostMelt = shadowAppearance =
        radioMelody = heartbeat = 0f;
        oxygenDrainFromNearest = 0f;
    }
}
