using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody rb;

    [Header("Movement")]
    public float moveForce = 15f;
    public float maxSpeed = 6f;

    void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal"); // A/D or Left/Right
        float v = Input.GetAxis("Vertical");   // W/S or Up/Down

        Vector3 inputDir = new Vector3(h, 0f, v);

        // Apply force in direction of input
        if (inputDir.sqrMagnitude > 0.001f)
        {
            rb.AddForce(inputDir.normalized * moveForce, ForceMode.Force);
        }

        // Optional: clamp speed so it doesn't accelerate forever
        Vector3 vel = rb.velocity;
        Vector3 horizontalVel = new Vector3(vel.x, 0f, vel.z);

        if (horizontalVel.magnitude > maxSpeed)
        {
            Vector3 limited = horizontalVel.normalized * maxSpeed;
            rb.velocity = new Vector3(limited.x, vel.y, limited.z);
        }
    }
}
