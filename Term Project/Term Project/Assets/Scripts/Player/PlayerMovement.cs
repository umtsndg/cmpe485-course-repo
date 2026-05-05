using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float groundMoveSpeed = 6f;
    public float airMoveSpeed = 4f;

    [Header("Jump Settings")]
    public float jumpForce = 7f;
    public float jumpHoldForce = 12f;
    public float maxJumpHoldTime = 0.2f;

    [Header("Dash Settings")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.2f;
    public LayerMask groundLayer;

    [Header("Visual References")]
    public PlayerLook playerLook;

    [Header("References")]
    public Transform orientation;

    private float horizontalInput;
    private float verticalInput;

    private Vector3 moveDirection;

    private Rigidbody rb;
    private bool isGrounded;

    private bool isJumping = false;
    private float jumpHoldTimer = 0f;

    private bool canDash = true;
    private bool isDashing = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        GetInput();
        CheckGround();

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isDashing)
        {
            Jump();
        }

        if (Input.GetKey(KeyCode.Space) && isJumping && !isDashing)
        {
            HoldJump();
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            isJumping = false;

            if (rb.velocity.y > 0f)
            {
                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y * 0.5f, rb.velocity.z);
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && !isDashing)
        {
            StartCoroutine(Dash());
        }
    }

    void FixedUpdate()
    {
        if (!isDashing)
        {
            MovePlayer();
        }
    }

    void GetInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    void CheckGround()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);

        if (isGrounded && rb.velocity.y <= 0f)
        {
            isJumping = false;
        }
    }

    void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        moveDirection = moveDirection.normalized;

        float currentSpeed = isGrounded ? groundMoveSpeed : airMoveSpeed;

        Vector3 targetVelocity = moveDirection * currentSpeed;
        Vector3 currentVelocity = rb.velocity;

        Vector3 velocityChange = new Vector3(
            targetVelocity.x - currentVelocity.x,
            0f,
            targetVelocity.z - currentVelocity.z
        );

        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    void Jump()
    {
        isJumping = true;
        jumpHoldTimer = 0f;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void HoldJump()
    {
        if (jumpHoldTimer < maxJumpHoldTime && rb.velocity.y > 0f)
        {
            rb.AddForce(Vector3.up * jumpHoldForce, ForceMode.Acceleration);
            jumpHoldTimer += Time.deltaTime;
        }
        else
        {
            isJumping = false;
        }
    }

    IEnumerator Dash()
    {
        if (playerLook != null)
        {
            playerLook.SetDashFOV(true);
        }

        canDash = false;
        isDashing = true;
        isJumping = false;

        Vector3 dashDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (dashDirection == Vector3.zero)
        {
            dashDirection = orientation.forward;
        }

        dashDirection.Normalize();

        float startTime = Time.time;

        while (Time.time < startTime + dashDuration)
        {
            rb.velocity = new Vector3(
                dashDirection.x * dashSpeed,
                rb.velocity.y,
                dashDirection.z * dashSpeed
            );

            yield return new WaitForFixedUpdate();
        }

        isDashing = false;

        if (playerLook != null)
        {
            playerLook.SetDashFOV(false);
        }

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }
}