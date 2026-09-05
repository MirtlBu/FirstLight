using UnityEngine;

public class ZeroGravityDrift : MonoBehaviour
{
    [Header("Rotation")]
    public Vector3 rotationSpeed = new Vector3(4f, 7f, 3f);
    public float speedVariation = 0.35f;

    [Header("Optional Drift")]
    public bool driftPosition;
    public Vector3 driftAmplitude = new Vector3(0.15f, 0.15f, 0.15f);
    public float driftFrequency = 0.2f;

    Vector3 _initialPosition;
    Vector3 _phase;
    float _speedMultiplier;

    void Awake()
    {
        _initialPosition = transform.localPosition;
        _phase = new Vector3(Random.value * 10f, Random.value * 10f, Random.value * 10f);
        _speedMultiplier = Random.Range(1f - speedVariation, 1f + speedVariation);
    }

    void Update()
    {
        transform.Rotate(Vector3.Scale(rotationSpeed, Vector3.one) * (_speedMultiplier * Time.deltaTime), Space.Self);

        if (driftPosition)
        {
            float time = Time.time * driftFrequency;
            Vector3 offset = new Vector3(
                Mathf.Sin(time + _phase.x),
                Mathf.Sin(time * 0.83f + _phase.y),
                Mathf.Sin(time * 0.71f + _phase.z));
            transform.localPosition = _initialPosition + Vector3.Scale(offset, driftAmplitude);
        }
    }
}
