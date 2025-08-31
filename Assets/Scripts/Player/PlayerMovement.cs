using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    Player playerScript;
    Rigidbody rb;
    [SerializeField] GameObject playerModel;
    [SerializeField] GameObject playerHurtbox;
    public Transform playerCam;
    PlayerSFXPlayer sfxPlayer;
    public bool playerActive = true;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 0f;
    [SerializeField] float moveSpeedDefault = 10f;
    [SerializeField] float moveSpeedSprint = 20f;
    [SerializeField] float moveSpeedDash = 100f;
    private float horizontalInput;
    private float verticalInput;
    private Vector3 moveDirection;
    Transform orientation;
    private Quaternion targetModelRotation;

    [Header("Jumping")]
    [SerializeField] GameObject jumpEffect;
    [SerializeField] float jumpForce = 11f; // 10
    [SerializeField] float jumpCooldown = 0.25f;
    [SerializeField] float airMultiplier = 0.4f;
    [SerializeField] float fallSpeed = 1.5f;
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
    private float sphereRadius = 1f; // sphere radius for SphereCast


    [Header("Sprinting")]
    [SerializeField] public bool isSprinting = false;

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
    public string sprintBtn = "Sprint1";

    //For whether the player is in the pause menu or not
    public bool inputActive = true;

    [SerializeField] private EventSystemUpdate myEventSystem;
    public bool isP1;
    private float navCooldown = 0.2f;
    private float lastNavTime = 0f;

    void Start()
    {
        playerScript = GetComponent<Player>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        orientation = gameObject.transform;
        jumpsLeft = maxJumps;

        sfxPlayer = GetComponent<PlayerSFXPlayer>();
        playerHurtbox = transform.Find("Hurtbox").gameObject;
        inputActive = true;

        //if (myEventSystem == null)
        //{
        //    string targetName = isP1 ? "EventSystemP1" : "EventSystemP2";
        //    GameObject obj = GameObject.Find(targetName);
        //    if (obj != null) myEventSystem = obj.GetComponent<EventSystemUpdate>();
        //}

        //if (isP1)
        //{
        //    Debug.Log("this is P1");
        //} else
        //{
        //    Debug.Log("this is p2");
        //}
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

            //Runs regular player movement input if inputActive is true(not in pause menu)
            if (inputActive)
            {
                PlayerInput();
            }
            else
            {
                Debug.Log("Input is inactive, not processing player input.");
            }


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
            {
                //targetModelRotation = Quaternion.LookRotation(moveDirection.normalized, Vector3.up);
                Vector3 camForward = playerCam.forward;
                camForward.y = 0;
                targetModelRotation = Quaternion.LookRotation(camForward, Vector3.up);
            }

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
            //moveSpeed = (horizontalInput != 0 || verticalInput != 0) ? moveSpeedDefault : 0f;
            if (horizontalInput != 0 || verticalInput != 0)
            {
                moveSpeed = (isSprinting) ? moveSpeedSprint : moveSpeedDefault;
            }
            else
            {
                moveSpeed = 0f;
                // cancel sprint
                isSprinting = false;
            }
        }

        if (Input.GetButtonDown(jumpBtn) && canJump)
        {
            //if (isSprinting) isSprinting = false;

            if (jumpsLeft > 0)
            {
                jumpsLeft--;
                Jump(false);
            }
        }

        if (Input.GetButtonDown(sprintBtn) && grounded)
        {
            Debug.Log("sprinting");
            isSprinting = true;
        }

        if (Input.GetButtonDown(dashBtn) && canDash && dashesLeft > 0)
        {
            if (isSprinting) isSprinting = false;

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
            {
                rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

                if (!grounded && rb.velocity.y < 0f)
                {
                    //rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y * 1.05f, rb.velocity.z);
                    rb.AddForce(Vector3.down * fallSpeed * 10f, ForceMode.Force);
                    //Debug.Log(rb.velocity.y);
                }
            }

        }
    }


    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }


    public void Jump(bool high)
    {
        grounded = false;
        hitGround = false;
        canJump = false;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (high)
            rb.AddForce(transform.up * (jumpForce * 3), ForceMode.Impulse);
        else
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

        playerScript.soundIndex = 0x01;
        sfxPlayer.PlaySFX(0);
        Instantiate(jumpEffect, transform.position, quaternion.identity);


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
        playerScript.soundIndex = 0x02;
        sfxPlayer.PlaySFX(1);
        dashRoutine = StartCoroutine(DashCoroutine());
    }


    private IEnumerator DashCoroutine()
    {
        canDash = false;
        isDashing = true;
        dashesLeft--;
        playerHurtbox.SetActive(false); // temporarily disable hurtbox

        Vector3 direction = moveDirection.normalized;
        if (direction == Vector3.zero) direction = playerModel.transform.forward;   // dash into facing direction if no movement

        Vector3 start = rb.position;
        Vector3 target = start + direction * maxDashDistance;

        //if (Physics.SphereCast(start, sphereRadius, direction, out RaycastHit hit, maxDashDistance, dashCollisionMask))
        if (Physics.Raycast(start, direction, out RaycastHit hit, maxDashDistance, dashCollisionMask))
        {
            target = hit.point - direction * dashStopPadding;
        }
        if (Physics.SphereCast(start, sphereRadius, direction, out RaycastHit hitG, maxDashDistance, groundLayer))
        {
            target.y = hitG.point.y + 2.1f; // slightly above ground to avoid clipping
        }

        float elapsed = 0f;
        float duration = dashDuration;

        if (disableGravityDuringDash) rb.useGravity = false;
        if (resetVelocityOnDash) rb.velocity = Vector3.zero;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            Vector3 horizontalPosition = Vector3.Lerp(start, target, t);

            // Adjust height to match the ground
            Vector3 rayOrigin = horizontalPosition + Vector3.up * 1.0f; // raise above ground
            if (Physics.SphereCast(rayOrigin, sphereRadius, Vector3.down, out RaycastHit groundHit, 2.1f, groundLayer))
            {
                horizontalPosition.y = groundHit.point.y + 2.1f; // slightly above ground to avoid clipping
                target.y = groundHit.point.y + 2.1f;

            }

            rb.MovePosition(horizontalPosition);
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        rb.MovePosition(target);

        if (disableGravityDuringDash) rb.useGravity = true;
        isDashing = false;
        playerHurtbox.SetActive(true); // re-enable hurtbox

        Invoke(nameof(ResetDash), dashCooldown);
    }

    // OLD DASH
    private IEnumerator DashCoroutine1()
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
