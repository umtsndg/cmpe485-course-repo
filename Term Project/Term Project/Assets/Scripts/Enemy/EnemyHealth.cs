using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public Animator animator;
    public EnemyRanged enemyRanged;

    private bool isDead = false;

    public void Die()
    {
        if (isDead) return;

        isDead = true;

        if (enemyRanged != null)
        {
            enemyRanged.MarkDead();
        }


        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        Destroy(gameObject, 2f);
    }
}