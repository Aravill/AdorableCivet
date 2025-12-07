using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
  [Header("Camera Settings")]
  [SerializeField] private Camera playerCamera;
  [SerializeField] private float mouseSensitivity = 100f;
  [SerializeField] private float maxLookUpAngle = 80f; // Prevents looking past overhead

  private float xRotation = 0f;

  void Start()
  {
    // Lock cursor for first-person experience
    Cursor.lockState = CursorLockMode.Locked;
  }

  void Update()
  {
    HandleMouseLook();
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