using UnityEngine;

public class PlayerPush : MonoBehaviour
{
    public float pushForce = 4f;

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;

        if (body == null || body.isKinematic)
            return;

        // Ignore pushing downward when landing on top
        if (hit.moveDirection.y < -0.3f)
            return;

        // Push based on player -> object direction, not only hit.moveDirection
        Vector3 pushDir = hit.transform.position - transform.position;
        pushDir.y = 0f;
        pushDir.Normalize();

        body.AddForce(pushDir * pushForce, ForceMode.Impulse);
    }
}