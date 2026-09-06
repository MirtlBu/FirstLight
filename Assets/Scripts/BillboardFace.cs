using UnityEngine;

// Attach to any quad/plane to always face the camera.
public class BillboardFace : MonoBehaviour
{
    void LateUpdate()
    {
        if (Camera.main == null) return;
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
                         Camera.main.transform.up);
    }
}
