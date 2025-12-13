using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
  [Header("Attack Settings")]
  [SerializeField] private bool isAttacking = false;
  [SerializeField] private Animator weaponAnimator;
  [SerializeField] private WeaponHandler weaponHandler;


  // Update is called once per frame
  void Update()
  {
    if (Mouse.current.leftButton.isPressed && !isAttacking)
    {
      Attack();
    }
  }

  void Awake()
  {

  }

  private void Attack()
  {
    isAttacking = true;
    weaponAnimator.SetBool("isAttacking", true);
    weaponHandler.Attack();
  }

  public void OnAttackComplete()
  {
    isAttacking = false;
    weaponAnimator.SetBool("isAttacking", false);
    weaponHandler.EndAttack();
  }
}