using UnityEngine;

// Plays a click sound at a rate proportional to signal intensity.
public class GeigerController : MonoBehaviour
{
    public AudioSource clickSource;
    public float       minInterval = 3f;
    public float       maxInterval = 0.15f;

    float _timer;

    public void SetRate(float intensity)
    {
        if (clickSource == null) return;
        float interval = Mathf.Lerp(minInterval, maxInterval, intensity);
        _timer -= Time.deltaTime;
        if (_timer <= 0f)
        {
            clickSource.PlayOneShot(clickSource.clip);
            _timer = interval;
        }
    }
}
