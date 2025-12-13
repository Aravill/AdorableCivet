using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

  [Header("Enemy Settings")]
  [SerializeField] private int health;

  [Header("Damage Feedback")]
  [SerializeField] private float flashDuration = 0.1f;
  [SerializeField] private AnimationCurve flashCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
  [SerializeField] private float damageCooldown = 1f;

  private bool _canTakeDamage = true;
  private Renderer _renderer;
  private Material _material;

  private ParticleSystem _damageParticleSystem;

  [Header("Rigidbody Settings")]
  private Rigidbody _rigidbody;
  private float _gravity = 9.81f;
  private bool _grounded = false;
  private Collider _collider;
  void Awake()
  {
    _renderer = GetComponent<Renderer>();
    _rigidbody = GetComponent<Rigidbody>();
    _collider = GetComponent<Collider>();
    _damageParticleSystem = GetComponent<ParticleSystem>();
    if (_renderer != null)
    {
      // Create material instance to avoid affecting other enemies
      _material = new Material(_renderer.material);
      _renderer.material = _material;
    }
    _rigidbody.freezeRotation = true;
  }

  // Update is called once per frame
  void Update()
  {
    CheckGrounded();
    HandleGravity();
    HandleDrag();
  }

  public void TakeDamage(int damage, float knockbackForce = 0f, Vector3 knockbackDirection = default)
  {
    if (!_canTakeDamage) return;
    health -= damage;
    StartCoroutine(FlashDamage());
    StartCoroutine(DamageCooldown());
    if (knockbackForce > 0f)
    {
      if (_rigidbody != null)
      {
        _rigidbody.AddForce(knockbackDirection.normalized * knockbackForce, ForceMode.Impulse);
      }
    }
    _damageParticleSystem.Play();
    if (health < 1)
    {
      Die();
    }
  }

  private void HandleGravity()
  {
    Vector3 verticalVelocity = new Vector3(0, _rigidbody.linearVelocity.y, 0);
    // Handle gravity separately
    if (!_grounded && verticalVelocity.y < _gravity)
    {
      float remainingVelocity = _gravity - verticalVelocity.magnitude;
      // Cap the force so we don't exceed max speed
      float forceToApply = Mathf.Min(_gravity, remainingVelocity);
      _rigidbody.AddForce(Vector3.down * forceToApply, ForceMode.Force);
    }
  }

  private void HandleDrag()
  {
    if (_grounded)
    {
      _rigidbody.linearDamping = 3f;
    }
    else
    {
      _rigidbody.linearDamping = 2.5f;
    }
  }

  private void Die()
  {
    Destroy(gameObject);
  }

  private void CheckGrounded()
  {
    _grounded = GroundChecker.IsGrounded(transform, _collider, LayerMask.NameToLayer("Enemy"));
  }

  private IEnumerator DamageCooldown()
  {
    _canTakeDamage = false;
    yield return new WaitForSeconds(damageCooldown);
    _canTakeDamage = true;
  }
  private IEnumerator FlashDamage()
  {
    if (_material != null)
    {
      // Store original values
      Color originalColor = _material.color;
      Color originalEmission = _material.GetColor("_EmissionColor");

      float elapsedTime = 0f;
      Color flashColor = Color.white;

      while (elapsedTime < flashDuration)
      {
        float t = flashCurve.Evaluate(elapsedTime / flashDuration);

        // Blend between original and flash color
        Color currentColor = Color.Lerp(originalColor, flashColor, t);
        _material.color = currentColor;

        // Optional: Add emission for glow effect
        _material.SetColor("_EmissionColor", flashColor * t);

        elapsedTime += Time.deltaTime;
        yield return null;
      }

      // Reset to original
      _material.color = originalColor;
      _material.SetColor("_EmissionColor", originalEmission);
    }
  }

}
