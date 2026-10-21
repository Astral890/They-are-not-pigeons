using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    public float speed = 5f;
    public InputActionReference move;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        move.action.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 input = move.action.ReadValue<Vector2>();
        Vector3 movement = new Vector3(input.x * speed, 0, input.y * speed);
        rb.linearVelocity= movement;
    }
}
