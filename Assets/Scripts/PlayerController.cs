using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float speed        = 20f;
    public float rollSpeed    = 60f;

    Rigidbody _rb;

    void Awake()
    {
        _rb                  = GetComponent<Rigidbody>();
        _rb.useGravity        = false;
        _rb.linearDamping    = 2f;
        _rb.angularDamping   = 5f;
    }

    void FixedUpdate()
    {
        Vector2 move = Vector2.zero;

        // Support both old and new input system via Keyboard fallback
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)   move.y += 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) move.y -= 1f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)  move.x -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) move.x += 1f;
        }

        Vector3 dir = (transform.right * move.x + transform.up * move.y).normalized;
        _rb.AddForce(dir * speed, ForceMode.Acceleration);
    }
}
