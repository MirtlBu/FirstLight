using System.Collections.Generic;
using UnityEngine;

public class LighthouseBeam : MonoBehaviour
{
    public static readonly List<BeamReflectionTarget> Targets = new List<BeamReflectionTarget>();

    [Header("Beam")]
    public Light beamLight;
    [Range(1f, 30f)] public float rotationPeriod = 15f;
    [Range(1f, 30f)] public float detectionAnglePadding = 2f;
    public bool rotateAroundWorldUp = true;

    void Awake()
    {
        if (beamLight == null)
            beamLight = GetComponentInChildren<Light>();

        if (beamLight != null)
            beamLight.enabled = true;
    }

    void Update()
    {
        if (beamLight == null) return;

        float degreesPerSecond = 360f / Mathf.Max(1f, rotationPeriod);
        transform.Rotate(rotateAroundWorldUp ? Vector3.up : transform.up, degreesPerSecond * Time.deltaTime, Space.World);

        for (int i = Targets.Count - 1; i >= 0; i--)
        {
            if (Targets[i] == null)
                Targets.RemoveAt(i);
            else
                Targets[i].SetBeamHit(IsInsideBeam(Targets[i]));
        }
    }

    bool IsInsideBeam(BeamReflectionTarget target)
    {
        Vector3 toTarget = target.transform.position - beamLight.transform.position;
        float distance = toTarget.magnitude;
        if (distance <= 0.001f || distance > beamLight.range)
            return false;

        float halfAngle = beamLight.spotAngle * 0.5f + detectionAnglePadding;
        float angle = Vector3.Angle(beamLight.transform.forward, toTarget);
        if (angle > halfAngle)
            return false;

        if (target.blockBeam && Physics.Raycast(beamLight.transform.position, toTarget.normalized, out RaycastHit hit, distance))
            return hit.transform.IsChildOf(target.transform);

        return true;
    }
}
