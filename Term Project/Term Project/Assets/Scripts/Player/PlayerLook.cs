using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public Transform playerBody;
    public float mouseSensitivity = 100f;

    [Header("Dash FOV")]
    public Camera playerCamera;
    public float normalFOV = 60f;
    public float dashFOV = 75f;
    public float fovSmoothSpeed = 8f;

    private float xRotation = 0f;
    private float yRotation = 0f;

    private bool isDashingVisual = false;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        yRotation = playerBody.eulerAngles.y;

        if (playerCamera != null)
        {
            playerCamera.fieldOfView = normalFOV;
        }
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        yRotation += mouseX;

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.rotation = Quaternion.Euler(0f, yRotation, 0f);

        UpdateFOV();
    }

    void UpdateFOV()
    {
        if (playerCamera == null) return;

        float targetFOV = isDashingVisual ? dashFOV : normalFOV;
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, Time.deltaTime * fovSmoothSpeed);
    }

    public void SetDashFOV(bool state)
    {
        isDashingVisual = state;
    }
}