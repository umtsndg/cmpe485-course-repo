using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float groundMoveSpeed = 6f;
    public float airMoveSpeed = 4f;

    [Header("Jump Settings")]
    public float jumpForce = 7f;
    public float jumpHoldForce = 30f;
    public float maxJumpHoldTime = 0.2f;
    public float jumpCutMultiplier = 0.5f;

    [Header("Dash Settings")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    [Header("Slide Settings")]
    public float slideSpeed = 12f;
    public float slideDuration = 0.6f;
    public KeyCode slideKey = KeyCode.LeftControl;

    [Header("Slide Camera")]
    public Transform playerCamera;
    public float slideCameraY = 0.9f;
    public float cameraSlideSmooth = 10f;

    [Header("Wall Run Settings")]
    public float wallCheckDistance = 0.7f;
    public LayerMask wallLayer;
    public float wallRunGravityScale = 0.3f;
    public float wallRunDuration = 1.2f;
    public float wallRunSpeed = 8f;
    public float wallJumpUpForce = 7f;
    public float wallJumpSideForce = 5f;
    public float wallRunCooldown = 0.25f;
    public float wallRunCameraTilt = 15f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.2f;
    public LayerMask groundLayer;

    [Header("References")]
    public Transform orientation;
    public PlayerLook playerLook;

    private float horizontalInput;
    private float verticalInput;
    private Vector3 moveDirection;

    private Rigidbody rb;
    private CapsuleCollider capsule;

    private bool isGrounded;
    private bool isJumping = false;
    private float jumpHoldTimer = 0f;

    private bool canDash = true;
    private bool isDashing = false;

    private bool isSliding = false;
    private float slideTimer;
    private Vector3 slideDirection;
    private float originalCapsuleHeight;
    private Vector3 originalCapsuleCenter;

    private Vector3 cameraOriginalLocalPos;
    private Vector3 cameraTargetLocalPos;

    private bool wallLeft;
    private bool wallRight;
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;

    private bool isWallRunning = false;
    private float wallRunTimer;
    private float wallRunCooldownTimer = 0f;

    private bool wasRunning;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();

        originalCapsuleHeight = capsule.height;
        originalCapsuleCenter = capsule.center;

        if (playerCamera != null)
        {
            cameraOriginalLocalPos = playerCamera.localPosition;
            cameraTargetLocalPos = cameraOriginalLocalPos;
        }
    }

    void Update()
    {
        GetInput();
        CheckGround();
        CheckWalls();
        HandleSlideCamera();
        HandleRunSound();

        if (wallRunCooldownTimer > 0f)
        {
            wallRunCooldownTimer -= Time.deltaTime;
        }

        // Start / stop wall run
        if (CanWallRun())
        {
            if (!isWallRunning)
            {
                StartWallRun();
            }
        }
        else
        {
            if (isWallRunning)
            {
                StopWallRun();
            }
        }

        // Jump / Wall Jump
        if (Input.GetKeyDown(KeyCode.Space) && !isDashing)
        {
            if (isWallRunning)
            {
                WallJump();
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayJump();
                }
            }
            else if (isGrounded)
            {
                if (isSliding)
                {
                    StopSlide();
                }

                Jump();
                Debug.Log("AudioManager.Instance: " + AudioManager.Instance);
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayJump();
                }
            }
        }

        // Variable jump cut
        if (Input.GetKeyUp(KeyCode.Space))
        {
            isJumping = false;

            if (rb.velocity.y > 0f)
            {
                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y * jumpCutMultiplier, rb.velocity.z);
            }
        }

        // Dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && !isDashing)
        {
            if (isSliding)
            {
                StopSlide();
            }

            if (isWallRunning)
            {
                StopWallRun();
            }

            StartCoroutine(Dash());
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayDash();
            }
        }

        // Slide
        if (Input.GetKeyDown(slideKey) && CanStartSlide())
        {
            StartSlide();
        }
    }

    void FixedUpdate()
    {
        if (isJumping && Input.GetKey(KeyCode.Space) && !isDashing && !isWallRunning)
        {
            HoldJump();
        }

        if (isWallRunning)
        {
            Debug.Log("Wall Running");
            HandleWallRun();
        }
        else if (isSliding)
        {
            HandleSlide();
        }
        else if (!isDashing)
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

    void CheckWalls()
    {
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallCheckDistance, wallLayer);
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallCheckDistance, wallLayer);
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
            jumpHoldTimer += Time.fixedDeltaTime;
        }
        else
        {
            isJumping = false;
        }
    }

    IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        isJumping = false;

        if (playerLook != null)
        {
            playerLook.SetDashFOV(true);
        }

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

        if (playerLook != null)
        {
            playerLook.SetDashFOV(false);
        }

        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    bool CanStartSlide()
    {
        Vector3 inputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        bool isMoving = inputDirection.magnitude > 0.1f;

        return isGrounded && isMoving && !isSliding && !isDashing && !isWallRunning;
    }

    void StartSlide()
    {
        isSliding = true;
        slideTimer = slideDuration;

        slideDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        slideDirection = slideDirection.normalized;

        if (slideDirection == Vector3.zero)
        {
            slideDirection = orientation.forward;
        }

        capsule.height = originalCapsuleHeight * 0.5f;
        capsule.center = new Vector3(originalCapsuleCenter.x, originalCapsuleCenter.y * 0.5f, originalCapsuleCenter.z);

        if (playerCamera != null)
        {
            cameraTargetLocalPos = new Vector3(
                cameraOriginalLocalPos.x,
                slideCameraY,
                cameraOriginalLocalPos.z
            );
        }
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySlide();
        }
    }

    void HandleSlide()
    {
        rb.velocity = new Vector3(
            slideDirection.x * slideSpeed,
            rb.velocity.y,
            slideDirection.z * slideSpeed
        );

        slideTimer -= Time.fixedDeltaTime;

        if (slideTimer <= 0f)
        {
            StopSlide();
        }
    }

    void StopSlide()
    {
        if (!isSliding) return;

        isSliding = false;
        capsule.height = originalCapsuleHeight;
        capsule.center = originalCapsuleCenter;

        if (playerCamera != null)
        {
            cameraTargetLocalPos = cameraOriginalLocalPos;
        }
    }

    void HandleSlideCamera()
    {
        if (playerCamera == null) return;

        playerCamera.localPosition = Vector3.Lerp(
            playerCamera.localPosition,
            cameraTargetLocalPos,
            Time.deltaTime * cameraSlideSmooth
        );
    }

    bool CanWallRun()
    {
        Vector3 inputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        bool isMoving = inputDirection.magnitude > 0.1f;

        bool wantsLeftWallRun = wallLeft && horizontalInput < 0f;
        bool wantsRightWallRun = wallRight && horizontalInput > 0f;

        return !isGrounded
            && isMoving
            && !isSliding
            && !isDashing
            && wallRunCooldownTimer <= 0f
            && (wantsLeftWallRun || wantsRightWallRun);
    }

    void StartWallRun()
    {
        isWallRunning = true;
        wallRunTimer = wallRunDuration;
        isJumping = false;

        if (playerLook != null)
        {
            playerLook.SetWallRunTilt(GetWallRunCameraTilt());
        }
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StartWallRunLoop();
        }
    }

    void StopWallRun()
    {
        if (!isWallRunning) return;

        isWallRunning = false;
        wallRunCooldownTimer = wallRunCooldown;

        if (playerLook != null)
        {
            playerLook.SetWallRunTilt(0f);
        }
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopLoop();
        }
    }

    float GetWallRunCameraTilt()
    {
        if (wallLeft)
        {
            return -wallRunCameraTilt;
        }

        if (wallRight)
        {
            return wallRunCameraTilt;
        }

        return 0f;
    }

    void HandleWallRun()
    {
        wallRunTimer -= Time.fixedDeltaTime;

        if (wallRunTimer <= 0f)
        {
            StopWallRun();
            return;
        }

        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y * wallRunGravityScale, rb.velocity.z);

        Vector3 wallForward = orientation.forward;

        if (wallLeft)
        {
            wallForward = Vector3.Cross(leftWallHit.normal, Vector3.up);
        }
        else if (wallRight)
        {
            wallForward = Vector3.Cross(Vector3.up, rightWallHit.normal);
        }

        if (Vector3.Dot(wallForward, orientation.forward) < 0f)
        {
            wallForward = -wallForward;
        }

        Vector3 targetVelocity = wallForward.normalized * wallRunSpeed;
        targetVelocity.y = rb.velocity.y;

        rb.velocity = targetVelocity;
    }

    void WallJump()
    {
        Debug.Log("Wall Jump");
        Vector3 wallNormal = Vector3.zero;

        if (wallLeft)
        {
            wallNormal = leftWallHit.normal;
        }
        else if (wallRight)
        {
            wallNormal = rightWallHit.normal;
        }

        StopWallRun();

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        Vector3 jumpForceDirection = Vector3.up * wallJumpUpForce + wallNormal * wallJumpSideForce;
        rb.AddForce(jumpForceDirection, ForceMode.Impulse);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }

        if (orientation != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, orientation.right * wallCheckDistance);
            Gizmos.DrawRay(transform.position, -orientation.right * wallCheckDistance);
        }
    }

    private void HandleRunSound()
    {
        bool isMoving = Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.1f ||
                        Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0.1f;

        bool shouldRunSoundPlay = isMoving && isGrounded;

        if (shouldRunSoundPlay && !wasRunning)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.StartRunLoop();
            }
        }
        else if (!shouldRunSoundPlay && wasRunning)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.StopLoop();
            }
        }

        wasRunning = shouldRunSoundPlay;
    }
}
