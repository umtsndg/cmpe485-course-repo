using UnityEngine;

public class EnemyMelee : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Animator animator;
    public Collider attackHitbox;

    [Header("Movement")]
    public float detectionRange = 10f;
    public float attackRange = 1.2f;
    public float moveSpeed = 3f;
    public float rotationSpeed = 5f;

    [Header("Attack Timing")]
    public float attackCooldown = 1.2f;
    public float totalAttackTime = 0.8f;

    [Header("Stun")]
    public float stunDuration = 2f;

    private float attackTimer = 0f;
    private bool isDead = false;
    private bool isAttacking = false;
    private bool isStunned = false;

    void Start()
    {
        if (attackHitbox != null)
        {
            attackHitbox.enabled = false;
        }
    }

    void Update()
    {
        if (isDead) return;
        if (isStunned) return;
        if (player == null) return;

        attackTimer -= Time.deltaTime;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > detectionRange)
        {
            if (animator != null)
            {
                animator.SetBool("IsRunning", false);
            }
            return;
        }

        RotateTowardPlayer();

        if (distanceToPlayer > attackRange && !isAttacking)
        {
            if (animator != null)
            {
                animator.SetBool("IsRunning", true);
            }

            MoveTowardPlayer();
        }
        else
        {
            if (animator != null)
            {
                animator.SetBool("IsRunning", false);
            }

            if (attackTimer <= 0f && !isAttacking)
            {
                StartAttack();
            }
        }
    }

    void MoveTowardPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0f;

        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    void RotateTowardPlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            Time.deltaTime * rotationSpeed
        );
    }

    void StartAttack()
    {
        isAttacking = true;
        attackTimer = attackCooldown;

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        Invoke(nameof(EndAttack), totalAttackTime);
    }

    public void EnableHitbox()
    {
        if (isDead || isStunned) return;

        if (attackHitbox != null)
        {
            attackHitbox.enabled = true;
        }
    }

    public void DisableHitbox()
    {
        if (attackHitbox != null)
        {
            attackHitbox.enabled = false;
        }
    }

    void EndAttack()
    {
        isAttacking = false;
        DisableHitbox();
    }

    public void MarkDead()
    {
        isDead = true;
        isAttacking = false;
        isStunned = false;

        DisableHitbox();
        CancelInvoke();

        if (animator != null)
        {
            animator.ResetTrigger("Attack");
            animator.ResetTrigger("Stun");
            animator.ResetTrigger("StunEnd");
            animator.SetBool("IsRunning", false);
            animator.SetBool("StunLoop", false);
        }
    }

    public void Stun()
    {
        if (isDead) return;
        if (isStunned) return;

        isStunned = true;
        isAttacking = false;

        DisableHitbox();
        CancelInvoke();

        if (animator != null)
        {
            animator.ResetTrigger("Attack");
            animator.SetBool("IsRunning", false);
            animator.SetTrigger("Stun");
        }

        Invoke(nameof(EnterStunLoop), 0.15f);
        Invoke(nameof(EndStun), stunDuration);
    }

    void EnterStunLoop()
    {
        if (isDead) return;
        if (!isStunned) return;

        if (animator != null)
        {
            animator.SetBool("StunLoop", true);
        }
    }

    void EndStun()
    {
        if (isDead) return;

        isStunned = false;

        if (animator != null)
        {
            animator.SetBool("StunLoop", false);
            animator.SetTrigger("StunEnd");
        }
    }

    public bool IsStunned()
    {
        return isStunned;
    }
}