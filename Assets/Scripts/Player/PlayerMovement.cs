using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
  [Header("Misc")]
  private CharacterController _charController;

  [Header("Camera Settings")]
  [SerializeField] private Camera playerCamera;
  [SerializeField] private float mouseSensitivity = 100f;
  [SerializeField] private float maxLookUpAngle = 80f; // Prevents looking past overhead


  [Header("Movement Settings")]
  [SerializeField] private float acceleration = 0.2f;
  [SerializeField] private float deceleration = 0.2f;
  [SerializeField] private float maxMoveSpeed = 1f;

  private PlayerInputActions _actions;
  private InputAction _movementAction;
  private float gravity = 9.81f;

  private float xRotation = 0f;

  void Start()
  {
    // Lock cursor for first-person experience
    Cursor.lockState = CursorLockMode.Locked;
  }

  void Awake()
  {
    _charController = GetComponent<CharacterController>();
    _actions = new PlayerInputActions();
    _movementAction = _actions.Movement.Move;
  }

  void Update()
  {
    HandleMouseLook();
    HandleMovement();
  }

  private void OnEnable()
  {
    _actions.Movement.Enable();
  }

  private void OnDisable()
  {
    _actions.Movement.Disable();
  }

  private void HandleMovement()
  {
    Vector2 input = _movementAction.ReadValue<Vector2>();

    Vector3 forward = playerCamera.transform.forward;
    Vector3 right = playerCamera.transform.right;

    forward.y = 0;
    right.y = 0;

    forward.Normalize();
    right.Normalize();

    Vector3 targetDirection = (forward * input.y + right * input.x).normalized;
    Vector3 targetVelocity = targetDirection * maxMoveSpeed;

    if (input.magnitude > 0)
    {
      // Accelerate towards target velocity
      targetDirection = Vector3.MoveTowards(_charController.velocity, targetVelocity, acceleration * Time.deltaTime);
    }
    else
    {
      // Decelerate to a stop
      targetDirection = Vector3.MoveTowards(_charController.velocity, Vector3.zero, deceleration * Time.deltaTime);
    }

    if (!_charController.isGrounded)
    {
      Vector3 gravityVector = Vector3.down * gravity * Time.deltaTime;
      targetDirection += gravityVector;
    }

    _charController.Move(targetDirection);
  }
  private void HandleMouseLook()
  {
    if (playerCamera == null)
    {
      Debug.Log("PlayerMovement: No camera assigned for mouse look.");
      return;
    }

    // Get mouse input
    float mouseX = Mouse.current.delta.x.ReadValue() * mouseSensitivity * Time.deltaTime;
    float mouseY = Mouse.current.delta.y.ReadValue() * mouseSensitivity * Time.deltaTime;

    // Rotate player body horizontally
    transform.Rotate(Vector3.up * mouseX);

    // Rotate camera vertically with clamping to prevent over-rotation
    xRotation -= mouseY;
    xRotation = Mathf.Clamp(xRotation, -maxLookUpAngle, maxLookUpAngle);
    playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
  }
}