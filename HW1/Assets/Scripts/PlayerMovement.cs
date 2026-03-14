using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform cameraTransform;
    public float speed = 5f;
    public float gravity = -9.81f;
    public float rotationSpeed = 10f;

    private Vector3 velocity;
    private Animator animator;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        bool grounded = controller.isGrounded;

        if (grounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 move = camForward * z + camRight * x;

        if (move.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        controller.Move(move * speed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        Vector3 localMove = transform.InverseTransformDirection(move);

        float moveX = localMove.x;
        float moveY = localMove.z;

        if (move.magnitude < 0.1f)
        {
            moveX = 0f;
            moveY = 0f;
        }

        animator.SetFloat("moveX", moveX, 0.1f, Time.deltaTime);
        animator.SetFloat("moveY", moveY, 0.1f, Time.deltaTime);
    }
}