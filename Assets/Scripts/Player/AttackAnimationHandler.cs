using UnityEngine;

public class AttackAnimationHandler : MonoBehaviour
{
  [SerializeField] private PlayerAttack playerAttack;

  public void OnAttackComplete()
  {
    if (playerAttack != null)
      playerAttack.OnAttackComplete();
  }
}
