using UnityEngine;

// Attach to any object that should emit signals: the real star and all false targets.
public class SignalEmitter : MonoBehaviour
{
    [Header("Configuration")]
    public SignalProfile profile;
    public float         maxRange      = 200f;
    public float         oxygenDrainPerSecond = 0f; // > 0 for hazardous false targets

    [HideInInspector] public float normalizedDistance = 1f; // 0 = at emitter, 1 = at maxRange

    void Update()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return;
        float dist = Vector3.Distance(transform.position, player.transform.position);
        normalizedDistance = Mathf.Clamp01(dist / maxRange);
    }
}
