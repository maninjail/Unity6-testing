using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 3f; //normal walking speed
    public float sprintSpeed = 6f; //speed when sprinting

    [Header("Rotation Settings")]
    public float rotationSpeed = 60f; //turn speed while walking
    public float sprintRotationSpeed = 40f; //turn speed while sprinting
    public float idleRotationSpeed = 80f; //turn speed while standing still

    private Rigidbody rb; //reference to the player's rigidbody
    private PlayerInputActions inputActions; //auto-generated input actions class
    private Vector2 moveInput; //stores movement input (stick/dpad/wasd)
    private float turnInput; //stores turn input (stick x/dpad/a/d)
    private bool isSprinting; //true if sprint input is held

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; //prevents unwanted physics rotation

        inputActions = new PlayerInputActions(); //initialize input actions

        //read movement input (stick or wasd/dpad)
        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += _ => moveInput = Vector2.zero;

        //read turn input (leftstick/dpad/keyboard)
        inputActions.Player.Turn.performed += ctx => turnInput = ctx.ReadValue<float>();
        inputActions.Player.Turn.canceled += _ => turnInput = 0f;

        //read sprint input (shift/l1/lb)
        inputActions.Player.Sprint.performed += _ => isSprinting = true;
        inputActions.Player.Sprint.canceled += _ => isSprinting = false;
    }

    void OnEnable() => inputActions.Enable(); //enable input actions when object is active
    void OnDisable() => inputActions.Disable(); //disable input actions when object is inactive

    void FixedUpdate()
    {
        //only allow sprint if input is mostly forward
        float vertical = isSprinting && !IsMostlyForward(moveInput) ? 0f : moveInput.y;

        //choose current speed
        float speed = isSprinting && vertical > 0 ? sprintSpeed : walkSpeed;

        //choose current rotation speed
        float currentRotationSpeed = vertical == 0
            ? idleRotationSpeed
            : (isSprinting ? sprintRotationSpeed : rotationSpeed);

        //apply turning
        if (Mathf.Abs(turnInput) > 0.1f)
        {
            float turnAmount = turnInput * currentRotationSpeed * Time.fixedDeltaTime;
            Quaternion turn = Quaternion.Euler(0f, turnAmount, 0f);
            rb.MoveRotation(rb.rotation * turn);
        }

        //calculate movement direction
        Vector3 moveDirection = transform.forward * vertical;
        Vector3 movement = moveDirection.normalized * speed;

        //apply movement while preserving y velocity
        rb.linearVelocity = new Vector3(movement.x, rb.linearVelocity.y, movement.z);

        //stop horizontal movement if not pressing forward/back
        if (Mathf.Abs(vertical) < 0.1f)
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
    }

    //returns true if stick is mostly pointing forward (within 60 degrees of up)
    bool IsMostlyForward(Vector2 input)
    {
        return input.magnitude > 0.1f && Vector2.Angle(Vector2.up, input.normalized) < 60f;
    }
}
