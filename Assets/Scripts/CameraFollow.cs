using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Settings")]
    public Vector3 offset    = new Vector3(0f, 2f, -15f);
    public float   smoothing = 5f;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desired = target.position + target.rotation * offset;
        transform.position = Vector3.Lerp(transform.position, desired, smoothing * Time.deltaTime);
        transform.LookAt(target.position);
    }
}
