using UnityEngine;

public class CameraOrbitCollision : MonoBehaviour
{
    public Transform target;

    public float mouseSensitivity = 3f;
    public float distance = 4f;
    public float minDistance = 1.0f;
    public float maxVerticalAngle = 35f;
    public float minVerticalAngle = -10f;

    public float targetHeight = 1.0f;
    public float collisionOffset = 0.2f;
    public LayerMask collisionMask;

    private float yaw = 0f;
    private float pitch = 8f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (target == null) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minVerticalAngle, maxVerticalAngle);

        Vector3 lookTarget = target.position + Vector3.up * targetHeight;

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 desiredOffset = rotation * new Vector3(0f, 0f, -distance);
        Vector3 desiredPosition = lookTarget + desiredOffset;

        Vector3 direction = desiredPosition - lookTarget;
        float desiredLength = direction.magnitude;
        direction.Normalize();

        float finalDistance = distance;

        RaycastHit hit;
        if (Physics.SphereCast(lookTarget, 0.2f, direction, out hit, desiredLength, collisionMask))
        {
            finalDistance = Mathf.Max(minDistance, hit.distance - collisionOffset);
        }

        Vector3 finalPosition = lookTarget + direction * finalDistance;

        transform.position = finalPosition;
        transform.LookAt(lookTarget);
    }
}