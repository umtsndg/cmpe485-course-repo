using System.Collections;
using UnityEngine;

public class SwingingTrunkTrap : MonoBehaviour
{
    [Header("Swing Settings")]
    public float leftAngle = -45f;
    public float rightAngle = 45f;
    public float swingDuration = 1f;
    public float pauseAtEnds = 0.2f;

    [Header("Hit Detection")]
    public CapsuleCollider trunkCollider;
    public LayerMask playerMask;

    private bool playerCaught = false;

    private void Start()
    {
        StartCoroutine(SwingRoutine());
    }

    private void Update()
    {
        CheckPlayerHit();
    }

    IEnumerator SwingRoutine()
    {
        while (true)
        {
            yield return StartCoroutine(RotateToAngle(leftAngle, rightAngle, swingDuration));
            yield return new WaitForSeconds(pauseAtEnds);

            yield return StartCoroutine(RotateToAngle(rightAngle, leftAngle, swingDuration));
            yield return new WaitForSeconds(pauseAtEnds);
        }
    }

    IEnumerator RotateToAngle(float startAngle, float endAngle, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            t = Mathf.SmoothStep(0f, 1f, t);

            float angle = Mathf.Lerp(startAngle, endAngle, t);
            transform.localRotation = Quaternion.Euler(0f, 0f, angle);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localRotation = Quaternion.Euler(0f, 0f, endAngle);
    }

    void CheckPlayerHit()
    {
        if (playerCaught || trunkCollider == null) return;

        Vector3 center = trunkCollider.bounds.center;
        Vector3 halfExtents = trunkCollider.bounds.extents;

        Collider[] hits = Physics.OverlapBox(
            center,
            halfExtents,
            trunkCollider.transform.rotation,
            playerMask,
            QueryTriggerInteraction.Ignore
        );

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player") || hit.transform.root.CompareTag("Player"))
            {
                playerCaught = true;

                GameManager gm = FindObjectOfType<GameManager>();
                if (gm != null)
                {
                    gm.FailGame();
                }

                break;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (trunkCollider == null) return;

        Gizmos.color = Color.red;
        Gizmos.matrix = Matrix4x4.TRS(
            trunkCollider.bounds.center,
            trunkCollider.transform.rotation,
            Vector3.one
        );
        Gizmos.DrawWireCube(Vector3.zero, trunkCollider.bounds.size);
    }
}