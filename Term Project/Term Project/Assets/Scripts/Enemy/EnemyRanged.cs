using UnityEngine;
using System.Collections;

public class EnemyRanged : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform firePoint;
    public GameObject projectilePrefab;
    public Animator animator;

    [Header("Settings")]
    public float detectionRange = 15f;
    public float attackCooldown = 2f;
    public float rotationSpeed = 5f;

    private float cooldownTimer = 0f;
    private bool isDead = false;

    void Update()
    {
        if (isDead) return;
        if (player == null) return;

        cooldownTimer -= Time.deltaTime;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            RotateTowardPlayer();

            if (cooldownTimer <= 0f)
            {
                Shoot();
                cooldownTimer = attackCooldown;
            }
        }
    }

    void RotateTowardPlayer()
    {
        if (isDead) return;

        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    void Shoot()
    {
        if (isDead) return;

        if (animator != null)
        {
            animator.SetTrigger("Shoot");
        }

        StartCoroutine(ShootBurst());
    }

    IEnumerator ShootBurst()
    {
        yield return new WaitForSeconds(0.1f);
        FireSingleProjectile();

        yield return new WaitForSeconds(0.12f);
        FireSingleProjectile();

        yield return new WaitForSeconds(0.12f);
        FireSingleProjectile();
    }

    void FireSingleProjectile()
    {
        if (isDead) return;
        if (projectilePrefab == null || firePoint == null || player == null) return;

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        EnemyProjectile projectileScript = projectile.GetComponent<EnemyProjectile>();
        if (projectileScript != null)
        {
            Vector3 direction = (player.position - firePoint.position).normalized;
            projectileScript.SetDirection(direction);
        }
    }

    public void MarkDead()
    {
        isDead = true;
    }
}