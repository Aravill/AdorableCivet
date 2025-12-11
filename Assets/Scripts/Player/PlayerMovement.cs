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
  private float gravity = 9.81f;
  private bool grounded = false;
  private float xRotation = 0f;

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
    Debug.Log(grounded ? "Grounded" : "Airborne");
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
    if (grounded && _jumpAction.triggered)
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
    if (!grounded && verticalVelocity.y < gravity)
    {
      float remainingVelocity = gravity - verticalVelocity.magnitude;
      // Cap the force so we don't exceed max speed
      float forceToApply = Mathf.Min(gravity, remainingVelocity);
      _rigidbody.AddForce(Vector3.down * forceToApply, ForceMode.Force);
    }
  }


  private void CheckGrounded()
  {
    RaycastHit hit;

    // Start raycast from bottom of player, not center
    Vector3 rayOrigin = transform.position;
    rayOrigin.y = _collider.bounds.min.y + 0.1f; // Slightly above the bottom of the collider
    Debug.Log("Ray Origin: " + rayOrigin);

    int layerMask = ~(1 << gameObject.layer); // Exclude player's own layer
    // Cast ray slightly longer than needed
    if (Physics.Raycast(rayOrigin, Vector3.down, out hit, 0.11f, layerMask))
    {
      if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Terrain"))
      {
        grounded = true;
      }
      else
      {
        grounded = false;
      }
    }
    else
    {
      grounded = false;
    }
  }

  private void HandleDrag()
  {
    if (grounded)
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