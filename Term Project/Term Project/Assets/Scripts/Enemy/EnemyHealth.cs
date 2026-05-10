using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public Animator animator;
    public EnemyMelee enemyMelee;
    public EnemyRanged enemyRanged;

    private bool isDead = false;

    public void Die()
    {
        if (isDead) return;

        isDead = true;

        bool diedWhileStunned = false;

        if (enemyMelee != null)
        {
            diedWhileStunned = enemyMelee.IsStunned();
            enemyMelee.MarkDead();
        }

        if (enemyRanged != null)
        {
            enemyRanged.MarkDead();
        }

        if (animator != null)
        {
            if (diedWhileStunned)
            {
                animator.SetTrigger("StunDie");
            }
            else
            {
                animator.SetTrigger("Die");
            }
        }

        Destroy(gameObject, 2f);
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayEnemyDie();
        }
    }
}