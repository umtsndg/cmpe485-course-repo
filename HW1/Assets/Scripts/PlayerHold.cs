using UnityEngine;

public class PlayerHold : MonoBehaviour
{
    public Camera playerCamera;
    public Transform holdPoint;
    public float interactDistance = 3f;

    [Header("Held Object Collision")]
    public float holdRadius = 0.2f;
    public float wallOffset = 0.05f;
    public LayerMask obstacleMask;

    private Rigidbody heldRb;
    private Transform heldObject;
    private Collider heldCollider;

    void Update()
    {
        Debug.DrawRay(
            playerCamera.transform.position,
            playerCamera.transform.forward * interactDistance,
            Color.red
        );

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldObject == null)
                TryPickUp();
            else
                DropObject();
        }

        if (heldObject != null)
        {
            UpdateHeldObjectPosition();
        }
    }

    void TryPickUp()
    {
        RaycastHit hit;

        if (Physics.Raycast(
            playerCamera.transform.position,
            playerCamera.transform.forward,
            out hit,
            interactDistance))
        {
            if (hit.transform.CompareTag("Key"))
            {
                heldObject = hit.transform;
                heldRb = heldObject.GetComponent<Rigidbody>();
                heldCollider = heldObject.GetComponent<Collider>();

                if (heldRb != null)
                {
                    heldRb.isKinematic = true;
                    heldRb.useGravity = false;
                }

                heldObject.SetParent(null);
            }
        }
    }

    void UpdateHeldObjectPosition()
    {
        Vector3 origin = playerCamera.transform.position;
        Vector3 targetPos = holdPoint.position;
        Vector3 dir = targetPos - origin;
        float dist = dir.magnitude;

        if (dist > 0.001f)
        {
            dir.Normalize();

            RaycastHit hit;
            if (Physics.SphereCast(origin, holdRadius, dir, out hit, dist, obstacleMask))
            {
                targetPos = origin + dir * Mathf.Max(0f, hit.distance - wallOffset);
            }
        }

        heldObject.position = targetPos;
        heldObject.rotation = holdPoint.rotation;
    }

    void DropObject()
    {
        if (heldObject == null) return;

        if (heldRb != null)
        {
            heldRb.isKinematic = false;
            heldRb.useGravity = true;
        }

        heldObject = null;
        heldRb = null;
        heldCollider = null;
    }
}