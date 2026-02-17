using UnityEngine;

public class ForceApplier : MonoBehaviour
{
    [Header("Target")]
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

    }
}
