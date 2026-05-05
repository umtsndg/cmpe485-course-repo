using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SwordAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackDuration = 0.2f;
    public float attackCooldown = 0.3f;

    [Header("Hit Timing")]
    public float hitDelay = 0.06f;
    public float hitActiveTime = 0.08f;
    public float hitCheckInterval = 0.02f;

    [Header("Attack Rotation")]
    public Vector3 idleRotation = Vector3.zero;
    public Vector3 attackRotation = new Vector3(0f, 0f, -80f);

    [Header("Hit Detection")]
    public Transform attackPoint;
    public float attackRadius = 1.5f;
    [Range(0f, 180f)] public float attackAngle = 90f;
    public LayerMask enemyLayer;

    [Header("Direction Reference")]
    public Transform attackDirectionSource;

    private bool canAttack = true;

    private Quaternion idleQuat;
    private Quaternion attackQuat;

    void Start()
    {
        idleQuat = Quaternion.Euler(idleRotation);
        attackQuat = Quaternion.Euler(attackRotation);
        transform.localRotation = idleQuat;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && canAttack)
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        canAttack = false;

        float halfDuration = attackDuration * 0.5f;
        float timer = 0f;
        float attackTimer = 0f;

        bool hitWindowStarted = false;
        HashSet<EnemyHealth> hitEnemiesThisAttack = new HashSet<EnemyHealth>();

        while (timer < halfDuration)
        {
            transform.localRotation = Quaternion.Slerp(idleQuat, attackQuat, timer / halfDuration);

            timer += Time.deltaTime;
            attackTimer += Time.deltaTime;

            if (!hitWindowStarted && attackTimer >= hitDelay)
            {
                hitWindowStarted = true;
                StartCoroutine(HitWindow(hitEnemiesThisAttack));
            }

            yield return null;
        }

        transform.localRotation = attackQuat;

        timer = 0f;
        while (timer < halfDuration)
        {
            transform.localRotation = Quaternion.Slerp(attackQuat, idleQuat, timer / halfDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        transform.localRotation = idleQuat;

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    IEnumerator HitWindow(HashSet<EnemyHealth> hitEnemiesThisAttack)
    {
        float timer = 0f;

        while (timer < hitActiveTime)
        {
            PerformHit(hitEnemiesThisAttack);
            timer += hitCheckInterval;
            yield return new WaitForSeconds(hitCheckInterval);
        }
    }

    void PerformHit(HashSet<EnemyHealth> hitEnemiesThisAttack)
    {
        if (attackPoint == null || attackDirectionSource == null)
            return;

        Collider[] hits = Physics.OverlapSphere(attackPoint.position, attackRadius, enemyLayer);

        foreach (Collider hit in hits)
        {
            EnemyHealth enemy = hit.GetComponent<EnemyHealth>();
            if (enemy == null)
                continue;

            if (hitEnemiesThisAttack.Contains(enemy))
                continue;

            Vector3 directionToEnemy = (hit.transform.position - attackDirectionSource.position).normalized;
            float angleToEnemy = Vector3.Angle(attackDirectionSource.forward, directionToEnemy);

            if (angleToEnemy <= attackAngle * 0.5f)
            {
                enemy.Die();
                hitEnemiesThisAttack.Add(enemy);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);

        if (attackDirectionSource != null)
        {
            Vector3 leftBoundary = Quaternion.Euler(0f, -attackAngle * 0.5f, 0f) * attackDirectionSource.forward;
            Vector3 rightBoundary = Quaternion.Euler(0f, attackAngle * 0.5f, 0f) * attackDirectionSource.forward;

            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(attackDirectionSource.position, leftBoundary * attackRadius);
            Gizmos.DrawRay(attackDirectionSource.position, rightBoundary * attackRadius);
            Gizmos.DrawRay(attackDirectionSource.position, attackDirectionSource.forward * attackRadius);
        }
    }
}