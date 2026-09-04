using UnityEngine;
using UnityEngine.Events;

public class OxygenSystem : MonoBehaviour
{
    public static OxygenSystem Instance { get; private set; }

    [Header("Settings")]
    public float maxOxygen    = 100f;
    public float drainPerSec  = 2f;   // baseline drain

    [Header("Events")]
    public UnityEvent         onDepleted;

    public float Oxygen         { get; private set; }
    public float NormalizedLeft => Oxygen / maxOxygen;

    bool _depleted;

    void Awake()
    {
        Instance = this;
        Oxygen   = maxOxygen;
    }

    void Update()
    {
        if (_depleted) return;

        float extra = PlayerSensor.Instance != null
            ? PlayerSensor.Instance.oxygenDrainFromNearest
            : 0f;

        Oxygen -= (drainPerSec + extra) * Time.deltaTime;
        Oxygen  = Mathf.Max(0f, Oxygen);

        if (Oxygen <= 0f)
        {
            _depleted = true;
            onDepleted?.Invoke();
        }
    }

    public void AddOxygen(float amount)
    {
        Oxygen = Mathf.Min(maxOxygen, Oxygen + amount);
    }
}
