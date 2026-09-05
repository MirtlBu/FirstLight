using System.Collections;
using UnityEngine;

// Attach to any GameObject in the scene.
// Animates celestial body renderers one by one at game start:
//   Phase 1: alpha 0 → 1
//   Phase 2: emission 0 → maxEmission
//   Phase 3: emission maxEmission → restEmission
public class CelestialIntro : MonoBehaviour
{
    [System.Serializable]
    public struct CelestialEntry
    {
        public Renderer renderer;
        [Tooltip("Delay before this object starts its animation")]
        public float    delay;
    }

    [Header("Objects (in order of appearance)")]
    public CelestialEntry[] objects;

    [Header("Timings (seconds)")]
    public float alphaDuration   = 2f;
    public float emissionUpDuration   = 1.5f;
    public float emissionDownDuration = 1f;

    [Header("Emission")]
    public float maxEmission  = 10f;
    public float restEmission = 5f;

    void Start()
    {
        StartCoroutine(RunIntro());
    }

    IEnumerator RunIntro()
    {
        foreach (var entry in objects)
        {
            if (entry.renderer == null) continue;

            if (entry.delay > 0f)
                yield return new WaitForSeconds(entry.delay);

            StartCoroutine(AnimateObject(entry.renderer));
        }
    }

    IEnumerator AnimateObject(Renderer rend)
    {
        var mpb       = new MaterialPropertyBlock();
        var mat       = rend.sharedMaterial;
        Color baseCol = mat != null ? mat.GetColor("_BaseColor")     : Color.white;
        Color baseEm  = mat != null ? mat.GetColor("_EmissionColor") : Color.white;
        // Normalized emission color (direction), we scale its magnitude
        Color emDir   = baseEm.maxColorComponent > 0f ? baseEm / baseEm.maxColorComponent : Color.white;

        rend.enabled = true;

        // Phase 1: alpha 0 → 1
        yield return Tween(alphaDuration, t =>
        {
            rend.GetPropertyBlock(mpb);
            mpb.SetColor("_BaseColor",     new Color(baseCol.r, baseCol.g, baseCol.b, t));
            mpb.SetColor("_EmissionColor", emDir * 0f);
            rend.SetPropertyBlock(mpb);
        });

        // Phase 2: emission 0 → maxEmission
        yield return Tween(emissionUpDuration, t =>
        {
            rend.GetPropertyBlock(mpb);
            mpb.SetColor("_EmissionColor", emDir * Mathf.Lerp(0f, maxEmission, t));
            rend.SetPropertyBlock(mpb);
        });

        // Phase 3: emission maxEmission → restEmission
        yield return Tween(emissionDownDuration, t =>
        {
            rend.GetPropertyBlock(mpb);
            mpb.SetColor("_EmissionColor", emDir * Mathf.Lerp(maxEmission, restEmission, t));
            rend.SetPropertyBlock(mpb);
        });
    }

    IEnumerator Tween(float duration, System.Action<float> onUpdate)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            onUpdate(Mathf.Clamp01(elapsed / duration));
            yield return null;
        }
    }
}
