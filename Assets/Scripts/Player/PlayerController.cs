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
    private Vector2 moveInput; //stores movement input (forward/backward)
    private float turnInput; //stores turn input (left/right)
    private bool isSprinting; //true if sprint input is held

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; //prevents unwanted physics rotation

        inputActions = new PlayerInputActions(); //initialize input actions

        //read movement input (stick or wasd)
        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += _ => moveInput = Vector2.zero;

        //read turn input (leftstick/dpad/keyboard
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
        //choose current move and turn speeds
        float speed = isSprinting ? sprintSpeed : walkSpeed;
        float currentRotationSpeed = moveInput.y == 0
            ? idleRotationSpeed
            : (isSprinting ? sprintRotationSpeed : rotationSpeed);

        //apply rotation first so direction updates instantly
     if (Mathf.Abs(turnInput) > 0.1f)
        {
            Quaternion turn = Quaternion.Euler(0f, turnInput * currentRotationSpeed * Time.fixedDeltaTime, 0f);
            rb.MoveRotation(rb.rotation * turn);
        }

            //use the updated rotation to calculate consistent forward movement
            Vector3 moveDirection = transform.forward * moveInput.y;
            Vector3 movement = moveDirection.normalized * speed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + movement);
    }
}
