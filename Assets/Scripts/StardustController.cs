using UnityEngine;

// Rotates and intensifies a particle system so dust flows toward the player from the star.
public class StardustController : MonoBehaviour
{
    public ParticleSystem dustParticles;

    public void SetFlow(float intensity, SignalEmitter nearest)
    {
        if (dustParticles == null || nearest == null) return;

        var emission = dustParticles.emission;
        emission.rateOverTime = Mathf.Lerp(5f, 80f, intensity);

        // Aim particles from star direction toward player
        Vector3 toPlayer = (transform.position - nearest.transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(toPlayer);
    }
}
