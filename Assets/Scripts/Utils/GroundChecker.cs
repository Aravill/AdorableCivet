using UnityEngine;

public static class GroundChecker
{
  public static bool IsGrounded(Transform transform, Collider collider, int excludeLayer, float extraHeight = 0.1f)
  {
    RaycastHit hit;

    // Start raycast from bottom of player, not center
    Vector3 rayOrigin = transform.position;
    rayOrigin.y = collider.bounds.min.y + extraHeight; // Slightly above the bottom of the collider

    int layerMask = ~(1 << excludeLayer); // Exclude specified layer
    // Cast ray slightly longer than needed
    if (Physics.Raycast(rayOrigin, Vector3.down, out hit, 0.11f, layerMask))
    {
      if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Terrain"))
      {
        return true;
      }
      else
      {
        return false;
      }
    }
    else
    {
      return false;
    }
  }
}