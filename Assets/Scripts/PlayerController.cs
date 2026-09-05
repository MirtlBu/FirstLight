using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 20f;
    public float damping = 3f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 0.5f;
    public float maxPitchAngle = 85f;

    Rigidbody _rb;
    float _yaw;
    float _pitch;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
        _rb.linearDamping = damping;
        _rb.angularDamping = 10f;
        _rb.freezeRotation = true;

        _yaw = transform.eulerAngles.y;
        _pitch = transform.eulerAngles.x;
    }

    void Update()
    {
        // Hold RMB (two-finger click on trackpad) to look around — cursor stays free
        if (Mouse.current != null && Mouse.current.rightButton.isPressed)
        {
            Vector2 delta = Mouse.current.delta.ReadValue();
            _yaw   += delta.x * mouseSensitivity;
            _pitch -= delta.y * mouseSensitivity;
            _pitch  = Mathf.Clamp(_pitch, -maxPitchAngle, maxPitchAngle);
        }

        transform.rotation = Quaternion.Euler(_pitch, _yaw, 0f);
    }

    void FixedUpdate()
    {
        Vector3 move = Vector3.zero;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) move += transform.forward;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) move -= transform.forward;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) move -= transform.right;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) move += transform.right;
        }

        if (move.sqrMagnitude > 0f)
            _rb.AddForce(move.normalized * moveSpeed, ForceMode.Acceleration);
    }
}
