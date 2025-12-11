using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerAttack : MonoBehaviour
{
  [Header("Attack Settings")]
  [SerializeField] private bool isAttacking = false;
  [SerializeField] private Animator weaponAnimator;

  [Header("Weapon Object")]
  [SerializeField] private GameObject weaponObj;
  
  private Collider _weaponCollider;

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
    _weaponCollider =  weaponObj.GetComponent<Collider>();  
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