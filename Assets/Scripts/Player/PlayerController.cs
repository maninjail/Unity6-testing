using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 3f;           // Normal walk speed
    public float sprintSpeed = 6f;         // Sprinting speed

    [Header("Rotation Settings")]
    public float rotationSpeed = 60f;          // Rotation speed while walking
    public float sprintRotationSpeed = 40f;    // Slower rotation while sprinting
    public float idleRotationSpeed = 80f;      // Faster rotation when standing still

    private Rigidbody rb;
    private float moveInput;
    private float turnInput;
    private bool isSprinting;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // We'll handle rotation manually
    }

    void Update()
    {
        // Forward/backward input (W/S or Up/Down)
        moveInput = Input.GetAxisRaw("Vertical");

        // Left/right input: calculate turn direction (-1, 0, 1)
        turnInput = (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) ? 1f : 0f)
                  - (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) ? 1f : 0f);

        // Check if sprinting
        isSprinting = Input.GetKey(KeyCode.LeftShift);
    }

    void FixedUpdate()
    {
        // Choose movement speed based on sprint state
        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;

        // Choose rotation speed based on movement/sprint state
        float currentRotationSpeed = idleRotationSpeed; // default to idle
        if (Mathf.Abs(moveInput) > 0.01f)
        {
            currentRotationSpeed = isSprinting ? sprintRotationSpeed : rotationSpeed;
        }

        // Rotate character if turning
        if (turnInput != 0f)
        {
            float turnAmount = turnInput * currentRotationSpeed * Time.fixedDeltaTime;
            Quaternion turnRotation = Quaternion.Euler(0f, turnAmount, 0f);
            rb.MoveRotation(rb.rotation * turnRotation);
        }

        // Move forward/backward
        Vector3 moveDirection = transform.forward * moveInput * currentSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + moveDirection);
    }
}
