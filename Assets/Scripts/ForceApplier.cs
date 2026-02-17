using UnityEngine;

public class ForceApplier : MonoBehaviour
{
    [Header("Rigidbody Reference (drag & drop here)")]
    public Rigidbody targetRb;

    [Header("Force Settings")]
    public Vector3 forceDirection = Vector3.forward;
    public float forceStrength = 5f;
    public ForceMode forceMode = ForceMode.Force;

    void Awake()
    {
        if (targetRb == null)
            targetRb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (targetRb == null) return;

        // Apply force every physics step
        Vector3 force = forceDirection.normalized * forceStrength;
        targetRb.AddForce(force, forceMode);
    }
}
