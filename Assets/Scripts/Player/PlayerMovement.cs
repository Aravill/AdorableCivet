using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
  [Header("Camera Settings")]
  [SerializeField] private Camera playerCamera;
  [SerializeField] private float mouseSensitivity = 100f;
  [SerializeField] private float maxLookUpAngle = 80f; // Prevents looking past overhead


  [Header("Movement Settings")]
  [SerializeField] private float acceleration = 3f;
  [SerializeField] private float maxSpeed = 5f;
  [SerializeField] private float jumpForce = 10f;
  [SerializeField] private float groundDrag = 3f;
  [SerializeField] private float airDrag = 2.5f;

  private PlayerInputActions _actions;
  private InputAction _movementAction;
  private InputAction _jumpAction;

  private Collider _collider;

  private Rigidbody _rigidbody;
  private float _gravity = 9.81f;
  private bool _grounded = false;
  private float _xRotation = 0f;

  void Start()
  {
    // Lock cursor for first-person experience
    Cursor.lockState = CursorLockMode.Locked;
  }

  void Awake()
  {
    _actions = new PlayerInputActions();
    _movementAction = _actions.Movement.Move;
    _jumpAction = _actions.Movement.Jump;
    _rigidbody = GetComponent<Rigidbody>();
    _rigidbody.freezeRotation = true;
    _collider = GetComponent<Collider>();
  }

  void Update()
  {
    CheckGrounded();
    HandleMouseLook();
    HandleHorizontalMovement();
    HandleJump();
    HandleDrag();
  }

  private void OnEnable()
  {
    _actions.Movement.Enable();
  }

  private void OnDisable()
  {
    _actions.Movement.Disable();
  }

  private void HandleJump()
  {
    if (_grounded && _jumpAction.triggered)
    {
      _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
  }
  private void HandleHorizontalMovement()
  {
    Vector2 input = _movementAction.ReadValue<Vector2>();
    Vector3 cameraDirection = playerCamera.transform.right * input.x + playerCamera.transform.forward * input.y;

    // Prevent vertical movement
    cameraDirection.y = 0;
    cameraDirection.Normalize();

    Vector3 horizontalVelocity = new Vector3(_rigidbody.linearVelocity.x, 0, _rigidbody.linearVelocity.z);
    Vector3 verticalVelocity = new Vector3(0, _rigidbody.linearVelocity.y, 0);
    // Handle horizontal movement
    if (input.magnitude > 0.1f && horizontalVelocity.magnitude < maxSpeed)
    {
      float remainingVelocity = maxSpeed - horizontalVelocity.magnitude;
      // Cap the force so we don't exceed max speed
      float forceToApply = Mathf.Min(acceleration, remainingVelocity);
      // Apply horizontal movement force
      _rigidbody.AddForce(cameraDirection * forceToApply, ForceMode.Force);
    }

    // Handle gravity separately
    if (!_grounded && verticalVelocity.y < _gravity)
    {
      float remainingVelocity = _gravity - verticalVelocity.magnitude;
      // Cap the force so we don't exceed max speed
      float forceToApply = Mathf.Min(_gravity, remainingVelocity);
      _rigidbody.AddForce(Vector3.down * forceToApply, ForceMode.Force);
    }
  }


  private void CheckGrounded()
  {
    _grounded = GroundChecker.IsGrounded(transform, _collider, LayerMask.NameToLayer("Player"));
  }

  private void HandleDrag()
  {
    if (_grounded)
    {
      _rigidbody.linearDamping = groundDrag;
    }
    else
    {
      _rigidbody.linearDamping = airDrag;
    }
  }
  private void HandleMouseLook()
  {
    if (playerCamera == null)
    {
      return;
    }

    // Get mouse input
    float mouseX = Mouse.current.delta.x.ReadValue() * mouseSensitivity * Time.deltaTime;
    float mouseY = Mouse.current.delta.y.ReadValue() * mouseSensitivity * Time.deltaTime;

    // Rotate player body horizontally
    transform.Rotate(Vector3.up * mouseX);

    // Rotate camera vertically with clamping to prevent over-rotation
    _xRotation -= mouseY;
    _xRotation = Mathf.Clamp(_xRotation, -maxLookUpAngle, maxLookUpAngle);
    playerCamera.transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
  }
}