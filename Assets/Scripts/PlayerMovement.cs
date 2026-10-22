using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    public float speed = 5f;
    private Vector3 moveDirection;
    public InputActionReference move;
    public InputActionReference interact;
    private FountainController currentFountain;

    private void OnEnable()
    {
        move.action.Enable();
        interact.action.Enable();
        interact.action.performed += OnInteract;
    }

    private void OnDisable()
    {
        move.action.Disable();
        interact.action.performed -= OnInteract;
        interact.action.Disable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Vector2 input = move.action.ReadValue<Vector2>();
        moveDirection = new Vector3(input.x, 0f, input.y);
    }

    private void FixedUpdate()
    {
        Vector3 normalizedInput = moveDirection.normalized;

        rb.linearVelocity = new Vector3(normalizedInput.x * speed, rb.linearVelocity.y, normalizedInput.z * speed);

    }

    public void setCurrentFountain(FountainController fountain)
    {
        currentFountain = fountain;
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (currentFountain != null)
        {
            currentFountain.Interact();
        }
    }
}