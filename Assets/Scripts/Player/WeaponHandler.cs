using UnityEngine;

public class WeaponHandler : MonoBehaviour
{

  [Header("Player Attack Handler")]
  [SerializeField] private PlayerAttack playerAttack;
  [SerializeField] private Camera playerCamera;

  [Header("Weapon Settings")]
  [SerializeField] private int damage = 10;
  [SerializeField] private float knockbackForce = 5f;

  private Collider _collider;

  void Awake()
  {
    _collider = GetComponent<Collider>();
    _collider.enabled = false;
  }

  public void Attack()
  {
    Debug.Log("Weapon Attack Triggered");
    _collider.enabled = true;
  }

  public void EndAttack()
  {
    Debug.Log("Weapon Attack Ended");
    _collider.enabled = false;
  }

  public void OnTriggerEnter(Collider other)
  {
    if (other.gameObject.CompareTag("Enemy"))
    {
      EnemyController controller = other.gameObject.GetComponent<EnemyController>();
      if (controller != null)
      {
        controller.TakeDamage(damage, knockbackForce, new Vector3(playerCamera.transform.forward.x, 0f, playerCamera.transform.forward.z).normalized);
      }
    }
  }
}
