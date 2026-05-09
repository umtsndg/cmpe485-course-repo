using UnityEngine;

public class EnemyMeleeHitbox : MonoBehaviour
{
    public EnemyMelee enemyMelee;

    private void OnTriggerEnter(Collider other)
    {
        PlayerBlock playerBlock = other.GetComponentInParent<PlayerBlock>();
        PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();

        if (playerBlock == null || playerHealth == null)
            return;

        Vector3 targetPosition;

        if (playerBlock.playerView != null)
        {
            targetPosition = playerBlock.playerView.position;
        }
        else
        {
            targetPosition = playerBlock.transform.position;
        }

        Vector3 incomingDirection = (targetPosition - enemyMelee.transform.position).normalized;

        BlockResult result = playerBlock.TryHandleIncomingHit(3, incomingDirection);

        if (result.wasBlocked)
        {
            if (result.wasParried && enemyMelee != null)
            {
                enemyMelee.Stun();
            }

            return;
        }

        playerHealth.Die();
    }
}