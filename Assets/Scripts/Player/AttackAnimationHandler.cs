using UnityEngine;

public class AttackAnimationHandler : MonoBehaviour
{
  [SerializeField] private PlayerAttack playerAttack;

  public void AttackStarted()
  {
    if (playerAttack != null)
      playerAttack.AttackStarted();
  }
  public void AttackCompleted()
  {
    if (playerAttack != null)
      playerAttack.AttackCompleted();
  }
  public void AnimationCompleted()
  {
    if (playerAttack != null)
      playerAttack.AnimationCompleted();
  }
}
