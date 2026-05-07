using UnityEngine;

public class EnemyMeleeHitbox : MonoBehaviour
{
    public EnemyMelee enemyMelee;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerBlock playerBlock = other.GetComponentInParent<PlayerBlock>();
        PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();

        Vector3 incomingDirection = transform.root.forward;

        if (playerBlock != null)
        {
            BlockResult result = playerBlock.TryHandleIncomingHit(3, incomingDirection);

            if (result.wasBlocked)
            {
                if (result.wasParried && enemyMelee != null)
                {
                    enemyMelee.Stun();
                }

                return;
            }
        }

        if (playerHealth != null)
        {
            playerHealth.Die();
        }
    }
}