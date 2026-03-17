using System.Collections;
using UnityEngine;

public class GuardPatrol : MonoBehaviour
{
    [Header("Patrol Points")]
    public Transform pointA;
    public Transform pointB;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float rotationSpeed = 5f;
    public float waitTimeAtPoint = 0.5f;
    public float arriveThreshold = 0.1f;

    [Header("Detection")]
    public Transform player;
    public float detectionDistance = 5f;
    public float fieldOfViewAngle = 120f;

    [Header("Optional")]
    public Transform visionForwardSource;
    public Animator animator;

    private bool goingToB = true;
    private bool playerCaught = false;
    private CharacterController playerCC;

    void Start()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (player != null)
            playerCC = player.GetComponent<CharacterController>();

        StartCoroutine(PatrolRoutine());
    }

    void Update()
    {
        if (!playerCaught)
        {
            CheckPlayerDetection();
        }
    }

    IEnumerator PatrolRoutine()
    {
        while (true)
        {
            Transform targetPoint = goingToB ? pointB : pointA;

            if (animator != null)
                animator.SetBool("isWalking", true);

            while (Vector3.Distance(transform.position, targetPoint.position) > arriveThreshold)
            {
                Vector3 direction = (targetPoint.position - transform.position).normalized;
                direction.y = 0f;

                if (direction.magnitude > 0.01f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(
                        transform.rotation,
                        targetRotation,
                        rotationSpeed * Time.deltaTime
                    );
                }

                transform.position += direction * moveSpeed * Time.deltaTime;
                yield return null;
            }

            transform.position = new Vector3(
                targetPoint.position.x,
                transform.position.y,
                targetPoint.position.z
            );

            if (animator != null)
                animator.SetBool("isWalking", false);

            yield return new WaitForSeconds(waitTimeAtPoint);

            goingToB = !goingToB;
        }
    }

    void CheckPlayerDetection()
    {
        if (player == null)
        {
            Debug.Log("Player reference is NULL");
            return;
        }

        Vector3 guardEye = transform.position + Vector3.up * 1.5f;

        Vector3 playerTarget;
        if (playerCC != null)
        {
            playerTarget = playerCC.bounds.center;
        }
        else
        {
            playerTarget = player.position + Vector3.up * 1.0f;
        }

        Vector3 toPlayer = playerTarget - guardEye;
        float distance = toPlayer.magnitude;

        Debug.DrawRay(guardEye, toPlayer.normalized * Mathf.Min(distance, detectionDistance), Color.yellow);

        if (distance > detectionDistance)
            return;

        Vector3 forwardDir = visionForwardSource != null ? visionForwardSource.forward : transform.forward;
        float angle = Vector3.Angle(forwardDir, toPlayer);

        if (angle > fieldOfViewAngle * 0.5f)
            return;

        RaycastHit hit;
        if (Physics.Raycast(guardEye, toPlayer.normalized, out hit, distance))
        {
            Debug.DrawRay(guardEye, toPlayer.normalized * hit.distance, Color.red);
            Debug.Log("Ray hit = " + hit.transform.name + " | Tag = " + hit.transform.tag);

            if (hit.transform.CompareTag("Player") || hit.transform.root.CompareTag("Player"))
            {
                Debug.Log("PLAYER DETECTED!");

                playerCaught = true;

                GameManager gm = FindObjectOfType<GameManager>();
                if (gm != null)
                {
                    gm.FailGame();
                }
            }
        }
        else
        {
            Debug.Log("Ray hit nothing");
        }
    }
}