using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent(typeof(CharacterController))]
public class marcoMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [FormerlySerializedAs("velocidadMovimiento")]
    public float moveSpeed = 5.0f;
    public float jumpHeight = 2.0f;
    public float gravity = -9.81f;

    [Header("Camera Settings")]
    [Tooltip("Assign the CameraPivot (parent of the camera)")]
    public Transform cameraPivot;
    public float mouseSensitivity = 0.1f;
    public float verticalLookLimit = 80.0f;

    [Header("Animation")]
    public Animator animator;

    private CharacterController controller;
    private Vector3 verticalVelocity;
    private bool isGrounded;
    private float xRotation = 0.0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Keyboard keyboard = Keyboard.current;
        Mouse mouse = Mouse.current;
        if (keyboard == null || mouse == null) return;

        // 1. CAMERA ROTATION
        Vector2 mouseDelta = mouse.delta.ReadValue() * mouseSensitivity;
        transform.Rotate(Vector3.up * mouseDelta.x);

        xRotation -= mouseDelta.y;
        xRotation = Mathf.Clamp(xRotation, -verticalLookLimit, verticalLookLimit);
        if (cameraPivot != null)
            cameraPivot.localRotation = Quaternion.Euler(xRotation, 0.0f, 0.0f);

        // 2. GROUND MOVEMENT
        isGrounded = controller.isGrounded;
        if (isGrounded && verticalVelocity.y < 0)
            verticalVelocity.y = -2f;

        float horizontalInput = (keyboard.dKey.isPressed ? 1f : 0f) - (keyboard.aKey.isPressed ? 1f : 0f);
        float verticalInput = (keyboard.wKey.isPressed ? 1f : 0f) - (keyboard.sKey.isPressed ? 1f : 0f);

        Vector3 direction = transform.right * horizontalInput + transform.forward * verticalInput;
        direction = Vector3.ClampMagnitude(direction, 1f);
        controller.Move(direction * moveSpeed * Time.deltaTime);

        // 3. JUMP AND GRAVITY
        if (keyboard.spaceKey.wasPressedThisFrame && isGrounded)
            verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravity);

        verticalVelocity.y += gravity * Time.deltaTime;
        controller.Move(verticalVelocity * Time.deltaTime);

        // 4. ANIMATION
        if (animator != null && animator.runtimeAnimatorController != null)
            animator.SetFloat("Speed", direction.magnitude, 0.1f, Time.deltaTime);
    }
}