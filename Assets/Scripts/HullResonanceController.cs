using UnityEngine;

public class HullResonanceController : MonoBehaviour
{
    public AudioSource hullHum;
    public float       minPitch = 0.8f;
    public float       maxPitch = 1.4f;

    public void SetIntensity(float t)
    {
        if (hullHum == null) return;
        hullHum.volume = Mathf.Lerp(0.1f, 0.8f, t);
        hullHum.pitch  = Mathf.Lerp(minPitch, maxPitch, t);
    }
}
