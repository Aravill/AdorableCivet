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
      StartAnimation();
    }
  }

  void Awake()
  {

  }
  private void StartAnimation()
  {
    isAttacking = true;
    weaponAnimator.SetBool("isAttacking", true);
  }

  public void AnimationCompleted()
  {
    isAttacking = false;
    weaponAnimator.SetBool("isAttacking", false);
  }

  public void AttackStarted()
  {
    weaponHandler.Attack();
  }
  public void AttackCompleted()
  {
    weaponHandler.EndAttack();
  }
}