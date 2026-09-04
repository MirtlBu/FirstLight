using UnityEngine;

// Central hub: reads PlayerSensor, drives all feedback systems.
// Attach to a manager GameObject in the scene.
public class FeedbackOrchestrator : MonoBehaviour
{
    [Header("References")]
    public AmbientLightController  ambientLight;
    public BreathingController     breathing;
    public HullResonanceController hullResonance;
    public VisorFrostController    visorFrost;
    public StardustController      stardust;
    public MusicLayerController    music;
    public GeigerController        geiger;

    void Update()
    {
        if (PlayerSensor.Instance == null) return;
        var s = PlayerSensor.Instance;

        ambientLight?.SetIntensity(s.colorTemperature);
        breathing?.SetEase(s.breathingEase, OxygenSystem.Instance?.NormalizedLeft ?? 1f);
        hullResonance?.SetIntensity(s.hullResonance);
        visorFrost?.SetMelt(s.visorFrostMelt);
        stardust?.SetFlow(s.stardustFlow, PlayerSensor.Instance.nearest);
        music?.SetIntensity(s.musicTone);
        geiger?.SetRate(s.geigerPing);
    }
}
