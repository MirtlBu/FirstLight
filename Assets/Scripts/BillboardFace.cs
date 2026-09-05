using UnityEngine;

// Attach to any quad/plane to always face the camera.
public class BillboardFace : MonoBehaviour
{
    void Update()
    {
        if (Camera.main != null)
            transform.rotation = Camera.main.transform.rotation;
    }
}
