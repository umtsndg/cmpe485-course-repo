using UnityEngine;

public class DebugTeleporter : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform keyObject;

    [Header("VideoObjects Points")]
    public Transform guardPoint;
    public Transform trapPoint;
    public Transform keyPoint;
    public Transform endPoint;

    [Header("Optional Offsets")]
    public Vector3 playerOffset = Vector3.zero;
    public Vector3 keyOffset = Vector3.zero;

    private CharacterController playerController;

    void Start()
    {
        if (player != null)
        {
            playerController = player.GetComponent<CharacterController>();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            TeleportPlayer(guardPoint);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            TeleportPlayer(trapPoint);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            TeleportPlayer(keyPoint);

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            TeleportPlayer(endPoint);
            TeleportKey(endPoint);
        }
    }

    void TeleportPlayer(Transform targetPoint)
    {
        if (player == null || targetPoint == null) return;

        if (playerController != null)
            playerController.enabled = false;

        player.position = targetPoint.position + playerOffset;

        if (playerController != null)
            playerController.enabled = true;
    }

    void TeleportKey(Transform targetPoint)
    {
        if (keyObject == null || targetPoint == null) return;

        Rigidbody rb = keyObject.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        keyObject.position = targetPoint.position + keyOffset;

        if (rb != null)
        {
            rb.isKinematic = false;
        }
    }
}