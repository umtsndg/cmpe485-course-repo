using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody rb;

    [Header("Movement")]
    public float moveForce = 15f;
    public float maxSpeed = 6f;

    [Header("Camera")]
    public Transform cameraTransform;

    void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal"); // A/D
        float v = Input.GetAxis("Vertical");   // W/S

        // Camera directions projected onto horizontal plane
        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 camRight = cameraTransform.right;
        camRight.y = 0f;
        camRight.Normalize();

        // Combine input with camera orientation
        Vector3 moveDir = camForward * v + camRight * h;

        if (moveDir.sqrMagnitude > 0.001f)
        {
            rb.AddForce(moveDir.normalized * moveForce, ForceMode.Force);
        }

        // Clamp speed
        Vector3 vel = rb.velocity;
        Vector3 horizontalVel = new Vector3(vel.x, 0f, vel.z);

        if (horizontalVel.magnitude > maxSpeed)
        {
            Vector3 limited = horizontalVel.normalized * maxSpeed;
            rb.velocity = new Vector3(limited.x, vel.y, limited.z);
        }
    }
}
