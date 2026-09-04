using UnityEngine;

// Crossfades between a tense drone and a warm resolving melody as intensity grows.
public class MusicLayerController : MonoBehaviour
{
    public AudioSource drone;
    public AudioSource melody;

    public void SetIntensity(float t)
    {
        if (drone  != null) drone.volume  = Mathf.Lerp(drone.volume,  1f - t, Time.deltaTime);
        if (melody != null) melody.volume = Mathf.Lerp(melody.volume, t,      Time.deltaTime);
    }
}
