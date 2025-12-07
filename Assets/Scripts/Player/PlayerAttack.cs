using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerAttack : MonoBehaviour
{
  [Header("Attack Settings")]
  [SerializeField] private bool isAttacking = false;
  [SerializeField] private Animator weaponAnimator;

  // Update is called once per frame
  void Update()
  {
    if (Mouse.current.leftButton.isPressed && !isAttacking)
    {
      Attack();
    }
  }

  private void Attack()
  {
    isAttacking = true;
    weaponAnimator.SetBool("isAttacking", true);
  }

  public void OnAttackComplete()
  {
    isAttacking = false;
    weaponAnimator.SetBool("isAttacking", false);
  }
}