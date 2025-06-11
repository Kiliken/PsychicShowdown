using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] GameObject playerModel;
    public Transform playerCam;
    PlayerSFXPlayer sfxPlayer;
    public bool playerActive = true;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 0f;
    [SerializeField] float moveSpeedDefault = 10f;
    [SerializeField] float moveSpeedDash = 100f;
    private float horizontalInput;
    private float verticalInput;
    private Vector3 moveDirection;
    Transform orientation;
    private Quaternion targetModelRotation;

    [Header("Jumping")]
    [SerializeField] float jumpForce = 10f;
    [SerializeField] float jumpCooldown = 0.25f;
    [SerializeField] float airMultiplier = 0.4f;
    private bool canJump = true;
    public int jumpsLeft = 3;
    public int maxJumps = 3;

    [Header("Dashing")]
    //[SerializeField] float dashForce = 10f;
    [SerializeField] float dashCooldown = 0.5f;
    private bool canDash = true;
    public int dashesLeft = 3;
    public int maxDashes = 3;
    [SerializeField] float dashCdTime = 3f; // 2
    public float dashCdTimer = 0f;

    [SerializeField] private float dashDuration = 0.2f; // 0.25
    [SerializeField] private bool disableGravityDuringDash = true;
    [SerializeField] private bool resetVelocityOnDash = true;
    [SerializeField] private float maxDashDistance = 10f;
    [SerializeField] private float dashStopPadding = 0.3f;  // 0.1
    [SerializeField] private LayerMask dashCollisionMask;

    private bool isDashing = false;
    private Coroutine dashRoutine;

    [Header("Ground Check")]
    public float playerHeight = 2f;
    public LayerMask groundLayer;
    [SerializeField] bool grounded = false;
    [SerializeField] bool hitGround = false;

    [Header("Slope Handling")]
    [SerializeField] float maxSlopeAngle = 40f;
    private RaycastHit slopeHit;

    public string moveXInput = "Horizontal1";
    public string moveYInput = "Vertical1";
    public string jumpBtn = "Jump1";
    public string dashBtn = "Dash1";


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        orientation = gameObject.transform;
        jumpsLeft = maxJumps;

        sfxPlayer = GetComponent<PlayerSFXPlayer>();
    }


    void Update()
    {
        if (playerActive)
        {
            grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f, groundLayer);

            if (grounded && !hitGround && canJump)
            {
                hitGround = true;
                jumpsLeft = maxJumps;
            }

            PlayerInput();

            if (dashesLeft != maxDashes)
            {
                if (dashCdTimer < dashCdTime)
                    dashCdTimer += Time.deltaTime;
                else
                {
                    dashesLeft++;
                    dashCdTimer = 0;
                }
            }
        }
    }


    private void FixedUpdate()
    {
        if (playerActive && !isDashing)
        {
            Move();
            SpeedControl();
            if (moveDirection != Vector3.zero)
                targetModelRotation = Quaternion.LookRotation(moveDirection.normalized, Vector3.up);
        }
    }


    private void LateUpdate()
    {
        playerModel.transform.rotation = Quaternion.Slerp(playerModel.transform.rotation, targetModelRotation, 10f * Time.deltaTime);
    }


    private void PlayerInput()
    {
        horizontalInput = Input.GetAxisRaw(moveXInput);
        verticalInput = Input.GetAxisRaw(moveYInput);

        if (!isDashing)
        {
            moveSpeed = (horizontalInput != 0 || verticalInput != 0) ? moveSpeedDefault : 0f;
        }

        if (Input.GetButtonDown(jumpBtn) && canJump)
        {
            if (jumpsLeft > 0)
            {
                jumpsLeft--;
                Jump();
            }
        }

        if (Input.GetButtonDown(dashBtn) && canDash && dashesLeft > 0)
        {
            Dash();
        }
    }


    private void Move()
    {
        Vector3 camForward = playerCam.forward;
        Vector3 camRight = playerCam.right;
        camForward.y = 0;
        camRight.y = 0;

        Vector3 forwardRelative = verticalInput * camForward;
        Vector3 rightRelative = horizontalInput * camRight;

        moveDirection = forwardRelative + rightRelative;

        // raycast from top and bottom to see if the player is colliding with an obstacle
        // only add force if not
        Vector3 start = rb.position + new Vector3(0, 0.5f, 0);
        Vector3 start2 = rb.position + new Vector3(0, -0.5f, 0);
        Vector3 direction = moveDirection.normalized;
        if (!Physics.Raycast(start, direction, out RaycastHit hit, 1f, dashCollisionMask) && !Physics.Raycast(start2, direction, out RaycastHit hit2, 1f, dashCollisionMask))
        {
            if (grounded)
                rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
            else
                rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    }


    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }


    private void Jump()
    {
        grounded = false;
        hitGround = false;
        canJump = false;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

        sfxPlayer.PlaySFX(0);

        Invoke(nameof(ResetJump), jumpCooldown);
    }


    private void ResetJump()
    {
        canJump = true;
    }


    private void Dash()
    {
        if (!canDash || isDashing) return;
        if (dashRoutine != null) StopCoroutine(dashRoutine);
        sfxPlayer.PlaySFX(1);
        dashRoutine = StartCoroutine(DashCoroutine());
    }


    private IEnumerator DashCoroutine()
    {
        canDash = false;
        isDashing = true;
        dashesLeft--;

        Vector3 direction = moveDirection.normalized;
        if (direction == Vector3.zero) direction = playerModel.transform.forward;   // dash into facing direction if no movement

        Vector3 start = rb.position;
        Vector3 target = start + direction * maxDashDistance;

        // raycast into dashing direction and check if there is an object
        if (Physics.Raycast(start, direction, out RaycastHit hit, maxDashDistance, dashCollisionMask))
        {
            target = hit.point - direction * dashStopPadding;
        }

        float elapsed = 0f;
        float duration = dashDuration;

        if (disableGravityDuringDash) rb.useGravity = false;
        if (resetVelocityOnDash) rb.velocity = Vector3.zero;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            Vector3 newPos = Vector3.Lerp(start, target, t);
            rb.MovePosition(newPos);
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        rb.MovePosition(target);

        if (disableGravityDuringDash) rb.useGravity = true;
        isDashing = false;

        Invoke(nameof(ResetDash), dashCooldown);
    }


    private void ResetDash()
    {
        canDash = true;
    }


    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }


    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }
}
