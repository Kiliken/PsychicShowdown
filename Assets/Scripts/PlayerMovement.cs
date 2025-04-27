using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    public bool playerActive = true;
    [Header("Movement")]
    private float moveSpeed = 0f;
    [SerializeField] float moveSpeedDefault = 10f;
    //public float speedModifier = 0f;
    private float horizontalInput;
    private float verticalInput;
    private Vector3 moveDirection;
    Transform orientation;

    [Header("Jumping")]
    [SerializeField] float jumpForce = 10f;
    [SerializeField] float jumpCooldown = 0.25f; //cooldown in-between
    [SerializeField] float airMultiplier = 0.4f;
    private bool canJump = true;
    int jumpsLeft = 3;
    [SerializeField] int maxJumps = 3;
    [SerializeField] float jumpCdTime = 2f; // recharge cooldown
    float jumpCdTimer = 0f;

    [Header("Dashing")]
    [SerializeField] float dashForce = 70f;
    [SerializeField] float dashCooldown = 0.5f;
    private bool canDash = true;
    int dashesLeft = 3;
    [SerializeField] int maxDashes = 3;
    [SerializeField] float dashCdTime = 3f;
    float dashCdTimer = 0f;

    [Header("Ground Check")]
    public float playerHeight = 2f;
    public LayerMask groundLayer;
    [SerializeField] bool grounded = false;
    [SerializeField] bool hitGround = false; // for jumping

    [Header("Slope Handling")]
    [SerializeField] float maxSlopeAngle = 40f;
    private RaycastHit slopeHit;
    //private bool exitingSlope;


    [Space(10)]
    Rigidbody rb;
    [SerializeField] GameObject playerModel;


    // Start is called before the first frame update
    void Start(){
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        orientation = gameObject.transform;

        jumpsLeft = maxJumps;
    }


    // Update is called once per frame
    void Update(){
        if(playerActive){
            grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f, groundLayer);
            
            // restore player jumps once they landed
            if(grounded && !hitGround && canJump){
                hitGround = true;
                jumpsLeft = maxJumps;
                Debug.Log("Jumps recharged");
            }

            PlayerInput();

            // if(jumpsLeft != maxJumps){
            //     if(jumpCdTimer < jumpCdTime){
            //         jumpCdTimer += Time.deltaTime;
            //     }
            //     else{
            //         jumpsLeft++;
            //         jumpCdTimer = 0;
            //         Debug.Log("Jump Count Restored: "+ jumpsLeft);
            //     }
            // }

            if(dashesLeft != maxDashes){
                if(dashCdTimer < dashCdTime){
                    dashCdTimer += Time.deltaTime;
                }
                else{
                    dashesLeft++;
                    dashCdTimer = 0;
                    Debug.Log("Dash restored: " + dashesLeft);
                }
            }
        }
    }


    private void FixedUpdate(){
        if(playerActive){
            Move();
            SpeedControl();
            if(moveDirection != Vector3.zero){
                RotatePlayer();
            }
        }
    }


    private void PlayerInput(){
        // set up for player 1 and 2 later
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if(horizontalInput != 0 || verticalInput != 0)
            moveSpeed = moveSpeedDefault;
        else
            moveSpeed = 0f;

        // removed grounded since there is a triple jump
        if (Input.GetButtonDown("Jump") && canJump){
            if(jumpsLeft > 0){
                jumpsLeft--;
                Debug.Log("Jump Count: "+ jumpsLeft);
                Jump();
            }
            else{
                Debug.Log("Max jumped");
            }
        }

        if(Input.GetButtonDown("DashK1") && canDash){
            if(dashesLeft > 0){
                dashesLeft--;
                Debug.Log("Dash Count: "+ dashesLeft);
                Dash();
            }
            else{
                Debug.Log("Max dashed");
            }
        }
    }


    private void Move(){
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }


    // controls the speed so that it doesn't go over the desired velocity
    private void SpeedControl(){
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void RotatePlayer()
    {
        float angle = Mathf.Atan2(horizontalInput, verticalInput);
        angle = angle * Mathf.Rad2Deg;
        playerModel.transform.eulerAngles = new Vector3(0, angle, 0);
    }

    private void Jump(){
        grounded = false;
        hitGround = false;
        canJump = false;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

        Invoke(nameof(ResetJump), jumpCooldown);
    }


    private void ResetJump()
    {
        //exitingSlope = false;
        canJump = true;
    }

    private void Dash(){
        canDash = false;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if(horizontalInput != 0 || verticalInput != 0)
            rb.AddForce(moveDirection.normalized * dashForce, ForceMode.Impulse);
        else
            rb.AddForce(transform.forward * dashForce * 2, ForceMode.Impulse);

        Invoke(nameof(ResetDash), dashCooldown);
    }

    private void ResetDash(){
        canDash = true;
    }


    // see if slope movement is needed later
    private bool OnSlope() {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f)) {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }


    private Vector3 GetSlopeMoveDirection() { 
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }
}
