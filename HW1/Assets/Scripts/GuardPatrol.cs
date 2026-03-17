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
    public float detectionDistance = 2f;
    public float fieldOfViewAngle = 60f;
    public LayerMask visionMask;

    [Header("Animation")]
    public Animator animator;

    private bool goingToB = true;
    private bool playerCaught = false;

    void Start()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        StartCoroutine(PatrolRoutine());
    }

    void Update()
    {
        if (!playerCaught)
        {
            CheckPlayerDetection();
        }
    }

    System.Collections.IEnumerator PatrolRoutine()
    {
        while (true)
        {
            Transform targetPoint = goingToB ? pointB : pointA;

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

            animator.SetBool("isWalking", false);

            yield return new WaitForSeconds(waitTimeAtPoint);

            goingToB = !goingToB;
        }
    }

    void CheckPlayerDetection()
    {
        if (player == null) return;

        Vector3 guardEye = transform.position + Vector3.up * 1.5f;
        Vector3 playerTarget = player.position + Vector3.up * 1.0f;

        Vector3 toPlayer = playerTarget - guardEye;
        float distance = toPlayer.magnitude;

        if (distance > detectionDistance)
            return;

        float angle = Vector3.Angle(transform.forward, toPlayer);

        if (angle > fieldOfViewAngle * 0.5f)
            return;

        RaycastHit hit;
        if (Physics.Raycast(
            guardEye,
            toPlayer.normalized,
            out hit,
            detectionDistance,
            visionMask,
            QueryTriggerInteraction.Ignore))
        {
            Debug.DrawRay(guardEye, toPlayer.normalized * hit.distance, Color.red);

            if (hit.transform.CompareTag("Player"))
            {
                playerCaught = true;

                GameManager gm = FindObjectOfType<GameManager>();
                if (gm != null)
                {
                    gm.FailGame();
                }
            }
        }
    }
}